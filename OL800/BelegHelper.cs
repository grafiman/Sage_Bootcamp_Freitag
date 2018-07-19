using Sagede.Core.Tools;
using Sagede.OfficeLine.Engine;
using Sagede.OfficeLine.Wawi.Basic;
using Sagede.OfficeLine.Wawi.BelegBasic;
using Sagede.OfficeLine.Wawi.BelegEngine;
using Sagede.OfficeLine.Wawi.BelegProxyEngine;
using Sagede.OfficeLine.Wawi.Services;
using Sagede.OfficeLine.Wawi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSDev.OfficeLine.Workshop
{
    /// <summary>
    /// Hilfsfunktionen zur Belegverwaltung
    /// </summary>
    public static class BelegHelper
    {
        public static Int32 CreateVKBeleg(Mandant mandant,
            string belegkennzeichen,
            string empfaenger,
            DateTime belegdatum, string artikelnummer, decimal menge)
        {
            using (var beleg = new Beleg(mandant, Erfassungsart.Verkauf))
            {
                if (!beleg.Initialize(belegkennzeichen, belegdatum.Date, (short)mandant.PeriodenManager.Perioden.Date2Periode(belegdatum.Date).Jahr))
                {
                    throw new BelegHelperException("Initialize: " + beleg.Errors.GetDescriptionSummary());
                }

                beleg.Bearbeiter = mandant.Benutzer.Name;
                beleg.Matchcode = "Erzeugt über Beleghelper";
                beleg.Liefertermin = beleg.Belegdatum.AddDays(7);

                if (!beleg.SetKonto(empfaenger, false))
                {
                    throw new BelegHelperException("SetKonto: " + beleg.Errors.GetDescriptionSummary());
                }

                var position = new BelegPosition(beleg);

                if (!position.Initialize(Positionstyp.Artikel))
                {
                    throw new BelegHelperException("Position.Initialize: " + beleg.Errors.GetDescriptionSummary());
                }

                if (!position.SetArtikel(artikelnummer, 0))
                {
                    throw new BelegHelperException("Position.SetArtikel: " + beleg.Errors.GetDescriptionSummary());
                }

                //Bei manuellen Preisen
                //position.Einzelpreis = 100;
                //position.IstEinzelpreisManuell = true;

                position.Menge = menge;
                position.RefreshBasismenge(true, 2);
                position.Calculate();

                beleg.Positionen.Add(position);

                beleg.ReadStandardTexte(true);
                beleg.Renumber();
                beleg.Calculate(true);

                if (!beleg.Validate(true))
                {
                    throw new BelegHelperException("Validate: " + beleg.Errors.GetDescriptionSummary());
                }

                if (!beleg.Save(false))
                {
                    throw new BelegHelperException("Save: " + beleg.Errors.GetDescriptionSummary());
                }

                return beleg.Handle;
            }
        }

        public static bool TransformRechnung(Mandant mandant, int vorID, DateTime appDate)
        {
            Beleg zielBeleg = null;
            Beleg quellBeleg = null;

            try
            {
                int belegHandle = 0;
                var qry = new StringBuilder();

                qry.AppendLine("SELECT KHKVKBelege.BelID AS BelegID,* FROM KHKVKBelege WITH(ReadUncommitted) ");
                qry.AppendLine("LEFT JOIN KHKVKBelegeVorgaenge WITH(ReadUncommitted)");
                qry.AppendLine("ON (KHKVKBelege.BelID=KHKVKBelegeVorgaenge.BelID AND KHKVKBelegeVorgaenge.Mandant=KHKVKBelege.Mandant)");
                qry.AppendLine("INNER JOIN KHKVKBelegarten WITH(ReadUncommitted)  ON KHKVKBelege.Belegkennzeichen = KHKVKBelegarten.Kennzeichen");
                qry.AppendLine($"WHERE KHKVKBelege.Mandant={mandant.Id} AND KHKVKBelegeVorgaenge.VorID={vorID}");
                qry.AppendLine("AND KHKVKBelegarten.Projekt = 0 ORDER BY KHKVKBelege.BelID DESC");

                using (var reader = mandant.MainDevice.GenericConnection.ExecuteReader(qry.ToString()))
                {
                    if (reader.Read())
                    {
                        belegHandle = reader.GetInt32("BelegID");
                    }
                }

                //TODO: wenn kein Beleg gefunden
                //TODO: Optimierung und Semaphoren
                quellBeleg = new Beleg(mandant, Erfassungsart.Verkauf);

                if (!quellBeleg.Load(belegHandle))
                {
                    throw new BelegHelperException("Quellbeleg konnte nicht geladen werden.");
                }

                zielBeleg = new Beleg(mandant, Erfassungsart.Verkauf);

                var belegkennzeichen = ConversionHelper.ToString(mandant.PropertyManager.GetValue(ManProperties.DefaultBelegart));
                if (string.IsNullOrWhiteSpace(belegkennzeichen))
                {
                    mandant.PropertyManager.UpdateProperty(ManProperties.DefaultBelegart, "VFR", 0, "Default Belegart VÜB");
                    belegkennzeichen = "VFR";
                }

                var istOK = zielBeleg.Transform(belegHandle, belegkennzeichen, appDate,
                    (short)mandant.PeriodenManager.Perioden.Date2Periode(appDate).Jahr, false, true);

                if (!istOK)
                {
                    throw new BelegHelperException("Fehler bei Belegübernahme");
                }

                zielBeleg.ZKDs = quellBeleg.ZKDs;

                // Positionen übernehmen
                var abfmessage = new Message();
                var positionen = zielBeleg.AddVorgangspositionen(vorID, true, abfmessage);

                positionen.ToList().ForEach(p =>
                {
                    p.RefreshBasismenge(true, 2);
                    p.Calculate();
                    zielBeleg.Positionen.Add(p);
                });

                zielBeleg.VKDruckprozess = ConversionHelper.ToInt32(mandant.PropertyManager.GetValue(ManProperties.DefaultDruckprozess));
                zielBeleg.Status = BelegDruckstatus.InDruck;

                if (!zielBeleg.Validate(true))
                {
                    throw new BelegHelperException(zielBeleg.Errors.GetDescriptionSummary());
                }

                if (!zielBeleg.Save(false))
                {
                    throw new BelegHelperException(zielBeleg.Errors.GetDescriptionSummary());
                }

                var druckberichte = mandant.MainDevice.Entities.DruckprozesseDruckbelege2.GetList(zielBeleg.VKDruckprozess, true);

                druckberichte.ToList().Where(d => d.Druckmodus == 3).ToList().ForEach(b =>
                {
                    var druckKennzeichen = mandant.MainDevice.Entities.DruckbelegeKennzeichen.CreateItem();
                    druckKennzeichen.BelID = zielBeleg.Handle;
                    druckKennzeichen.DruckbelegTAN = b.TAN;
                    druckKennzeichen.Mandant = mandant.Id;
                    druckKennzeichen.BerichtName = b.BerichtName;
                    druckKennzeichen.Mitarbeiter = zielBeleg.Bearbeiter;
                    druckKennzeichen.Datum = DateTime.Now;
                    druckKennzeichen.Druckindex = b.Druckindex;
                    druckKennzeichen.Druckkennzeichen = 6;
                    druckKennzeichen.Exemplare = b.Exemplare;
                    druckKennzeichen.Typ = 4000;
                    druckKennzeichen.Save();
                });

                #region BelegServices Variante (funktioniert nicht -> Abstimmung mit SWE)

                //var services = new BelegServices();
                //var kennzeichen = string.Empty;

                //beleg = services.TransformVorgang(mandant, appDate, vorID, "VFR", Erfassungsart.Verkauf);
                //var dtoBeleg = BelegHandler.ToDataContainer(mandant, beleg);

                //kennzeichen = beleg.Kennzeichen ;

                //beleg.Dispose();
                //beleg = null;
                //beleg = new Beleg(mandant, Erfassungsart.Verkauf, true);
                //beleg.SetHandles = true;
                //beleg = BelegHandler.FromDataContainer(mandant,  appDate, dtoBeleg);

                //// TODO: in Konfig?!?
                //beleg.VKDruckprozess = 6;
                //beleg.Status = BelegDruckstatus.InDruck;

                //beleg.PrepareForDataService();
                //// TODO: hier noch einbauen => Stapeldruckzeugs
                //if (!beleg.Validate(true))
                //{
                //    throw new BelegHelperException("Validate: " + beleg.Errors.GetDescriptionSummary());
                //}

                //if (!beleg.Save(true))
                //{
                //    throw new BelegHelperException("Save: " + beleg.Errors.GetDescriptionSummary());
                //}

                #endregion BelegServices Variante (funktioniert nicht -> Abstimmung mit SWE)

                if (zielBeleg.Handle != 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                TraceLog.LogException(ex);
                return false;
                //throw new BelegHelperException("Fehler bei Belegübernahme.", ex);
            }
            finally
            {
                if (zielBeleg != null)
                {
                    zielBeleg.Dispose();
                    zielBeleg = null;
                }
                if (quellBeleg != null)
                {
                    quellBeleg.Dispose();
                    quellBeleg = null;
                }
            }
        }
    }
}