using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSDev.OfficeLine.Academy.BusinessLogic;
using PSDev.OfficeLine.Academy.DataAccess;
using Sagede.Core.Data;
using Sagede.Core.Tools;
using Sagede.OfficeLine.Data;
using Sagede.OfficeLine.Shared.Customizing;

namespace PSDev.OfficeLine.Academy.DCM
{
    public class DcmHandler : Sagede.OfficeLine.Shared.Customizing.IDcmCallback
    {
        public bool Entry(IDcmContext context)
        {
            try
            {
                switch (context.ListId)
                {

                    case DcmDefinitionManager.DcmListId.VKBelegPositionProxyBelegPositionToContainerPosition:
                        handleBelegPositionToContainerposition(context);
                        break;

                    case DcmDefinitionManager.DcmListId.VKBelegPositionProxyContainerPositionToBelegPosition:
                        handleContainerPositionToBelegPosition(context);
                        break;

                    case DcmDefinitionManager.DcmListId.VKBelegLoad:
                        handleVKBelegLoad(context);
                        break;

                    case DcmDefinitionManager.DcmListId.VKBelegSave:
                        handleVKBelegSave(context);
                        break;

                    case DcmDefinitionManager.DcmListId.VKBelegDelete:
                        var vkBelegDeleteContext = (Sagede.OfficeLine.Wawi.BelegEngine.DcmContextBeleg)context;
                  
                        break;

                    // Die Teilnehmer schreiben
                    case DcmDefinitionManager.DcmListId.PrintVKAddBelegPosition:
                        FillTempTable(context);
                        break;
                    // Die temporäre Tabelle leeren
                    case DcmDefinitionManager.DcmListId.PrintVKInitTables:
                        ClearTempTable(context);
                        break;
                    case DcmDefinitionManager.DcmListId.PrintVKUpdateTables:
                        CompleteTempTable(context);
                        break;

                    default:
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                TraceLog.LogException(ex);
                return false;
            }




        }

        private static void handleVKBelegSave(IDcmContext context)
        {
            var vkBelegSaveContext = (Sagede.OfficeLine.Wawi.BelegEngine.DcmContextBeleg)context;

            vkBelegSaveContext.Beleg.Positionen.ForEach(p =>
            {
                p.ExecuteSeminarbuchungen(vkBelegSaveContext.Mandant);
            });

            vkBelegSaveContext.Beleg.PositionenDeleted.ForEach(p =>
            {
                p.DeleteSeminarbuchungen(vkBelegSaveContext.Mandant);
            });
        }

        private static void handleBelegPositionToContainerposition(IDcmContext context)
        {
            var myContext = (Sagede.OfficeLine.Wawi.BelegProxyEngine.DcmContextBelegPositionProxyBelegPositionToContainerPosition)context;
            myContext.DataContainerPosition.SetChild<DataContainerSet>(myContext.Position.BelegPositionToContainer(), DcmHelper.RelationSeminarbuchungen);
        }

        private static void handleContainerPositionToBelegPosition(IDcmContext context)
        {
            var myContext2 = (Sagede.OfficeLine.Wawi.BelegProxyEngine.DcmContextBelegPositionProxyContainerPositionToBelegPosition)context;
            var seminarbuchungen = new Seminarbuchungen();
            seminarbuchungen.FromDataContainer(myContext2.DataContainerPosition);
            myContext2.Position.DCMProperties.ObjectValues[DcmHelper.RelationSeminarbuchungen] = seminarbuchungen;
        }

        private static void handleVKBelegLoad(IDcmContext context)
        {
            var vkBelegLoadContext = (Sagede.OfficeLine.Wawi.BelegEngine.DcmContextBeleg)context;
            var manager = new SeminarbuchungManager(vkBelegLoadContext.Mandant);

            vkBelegLoadContext.Beleg.Positionen.Where(p => p.Positionstyp == Sagede.OfficeLine.Wawi.Tools.Positionstyp.Artikel).ToList().ForEach(p =>
            {
                var buchungen = manager.GetBuchungen(p.VorgangspositionsHandle);
                if (buchungen.Count > 0)
                {
                    p.DCMProperties.ObjectValues[DcmHelper.RelationSeminarbuchungen] = buchungen;
                }
            });
        }

        /// <summary>
        /// Füllen der temporären Tabelle, der tKHKPrintMain und setzen des CurrentTableItemIndex
        /// </summary>
        /// <param name="context"></param>
        private void FillTempTable(IDcmContext context)
        {
            var printContext = (Sagede.OfficeLine.Wawi.PrintEngine.DcmContextBelegdruck)context;
            var mandant = printContext.Mandant;
            var beleg = printContext.Beleg;
            String query;
            IGenericCommand command;

            if (mandant.MainDevice.Lookup.GetInt16("PrintPosType", "tKHKPrintMain", String.Format("ConnID={0} AND ID={1}", printContext.ConnectionID, printContext.CurrentTableItemIndex)) == 21)
            {
                var parameterlist = new QueryParameterList();

                parameterlist.AddClauseParameter(new ClauseParameter() { ComparisonType = ClauseParameterComparisonType.Equals, FieldName = "ID", Value = printContext.CurrentTableItemIndex });
                parameterlist.AddClauseParameter(new ClauseParameter() { ComparisonType = ClauseParameterComparisonType.Equals, FieldName = "ConnID", Value = printContext.ConnectionID });

                var seminarArtikel = mandant.MainDevice.Entities.TempPrintPositionArtikelVK.GetItem(parameterlist);

                // Hier die Buchungen zu dem Seminarartikel heraussuchen und in die temporäre Drucktabelle einfügen
                query = @"SELECT SeminarTerminID, Adresse, AnsprechpartnerVorname, AnsprechpartnerNachname, AnsprechpartnerEmail FROM PSDSeminarbuchungen
                                WHERE BelPosId=@BelPosID AND Mandant=@Mandant";

                command = printContext.Mandant.MainDevice.GenericConnection.CreateSqlStringCommand(query);
                command.AppendInParameter("Mandant", typeof(Int16), printContext.Mandant.Id);
                command.AppendInParameter("BelPosID", typeof(Int32), seminarArtikel.BelPosID);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        printContext.CurrentTableItemIndex++;

                        var qry = new StringBuilder();

                        qry.AppendLine("INSERT INTO ");
                        qry.AppendLine("tPSDPrintTeilnehmer ");
                        qry.AppendLine("(ConnID, ID, InfodruckId, InfodruckDatum, BelPosID, VorPosId, SeminarterminId, Adresse, Vorname, Nachname, Email) ");
                        qry.AppendLine("VALUES (@ConnId, @TableItemIndex, @InfodruckHandle, @InfodruckDatum, @BelPosID, @VorPosID, ");
                        qry.AppendLine("@SeminarterminId, @Adresse, @AnsprechpartnerVorname, @AnsprechpartnerNachname, @AnsprechpartnerEmail)");

                        var commandTeilnehmer = mandant.MainDevice.GenericConnection.CreateSqlStringCommand(qry.ToString());

                        commandTeilnehmer.AppendInParameter("ConnID", typeof(int), printContext.ConnectionID);
                        commandTeilnehmer.AppendInParameter("TableItemIndex", typeof(int), printContext.CurrentTableItemIndex);
                        commandTeilnehmer.AppendInParameter("InfodruckHandle", typeof(int), printContext.InfodruckHandle);
                        commandTeilnehmer.AppendInParameter("InfodruckDatum", typeof(DateTime), printContext.InfodruckDatum);
                        commandTeilnehmer.AppendInParameter("BelPosID", typeof(int), seminarArtikel.BelPosID);
                        commandTeilnehmer.AppendInParameter("VorPosID", typeof(int), seminarArtikel.VorPosID);
                        commandTeilnehmer.AppendInParameter("SeminarterminId", typeof(string), reader.GetString("SeminarterminId"));
                        commandTeilnehmer.AppendInParameter("Adresse", typeof(int), reader.GetInt32("Adresse"));
                        commandTeilnehmer.AppendInParameter("AnsprechpartnerVorname", typeof(string), reader.GetString("AnsprechpartnerVorname"));
                        commandTeilnehmer.AppendInParameter("AnsprechpartnerNachname", typeof(string), reader.GetString("AnsprechpartnerNachname"));
                        commandTeilnehmer.AppendInParameter("AnsprechpartnerEmail", typeof(string), reader.GetString("AnsprechpartnerEmail"));

                        commandTeilnehmer.ExecuteNonQuery();

                        var qryPrintMain = new StringBuilder();

                        qryPrintMain.AppendLine("INSERT INTO ");
                        qryPrintMain.AppendLine("tKHKPrintMain ");
                        qryPrintMain.AppendLine("(ConnID, ID, InfodruckId, InfodruckDatum, PrintPosType, Seitenumbruch, Anzahlung) ");
                        qryPrintMain.AppendLine("VALUES (@ConnId, @TableItemIndex, @InfodruckHandle, @InfodruckDatum, 2500, 0, 0)");

                        var commandPrintMain = mandant.MainDevice.GenericConnection.CreateSqlStringCommand(qryPrintMain.ToString());

                        commandPrintMain.AppendInParameter("ConnID", typeof(int), printContext.ConnectionID);
                        commandPrintMain.AppendInParameter("TableItemIndex", typeof(int), printContext.CurrentTableItemIndex);
                        commandPrintMain.AppendInParameter("InfodruckHandle", typeof(int), printContext.InfodruckHandle);
                        commandPrintMain.AppendInParameter("InfodruckDatum", typeof(DateTime), printContext.InfodruckDatum);

                        commandPrintMain.ExecuteNonQuery();
                    }
                }
            }
        }

        // Löschen der temporären Daten für die ConnectionID
        private void ClearTempTable(IDcmContext context)
        {
            var printContext = (Sagede.OfficeLine.Wawi.PrintEngine.DcmContextBelegdruck)context;
            var mandant = printContext.Mandant;
            var beleg = printContext.Beleg;
            String clause;

            if (printContext.IstPrepareInfoDruck == true)
            {
                clause = String.Format("InfodruckId > 0 AND InfodruckDatum < CONVERT(DATETIME, '{0}',104)", ConversionHelper.ToString(printContext.InfodruckDatumDelete));
            }
            else
            {
                clause = String.Format("ConnID = {0}", printContext.ConnectionID);
            }

            mandant.MainDevice.GenericConnection.ExecuteNonQuery("DELETE FROM tPSDPrintTeilnehmer WHERE " + clause);
        }

        private void CompleteTempTable(IDcmContext context)
        {
            var printContext = (Sagede.OfficeLine.Wawi.PrintEngine.DcmContextBelegdruck)context;
            var mandant = printContext.Mandant;
            String clause;

            clause = " WHERE ConnID = 0 AND InfodruckId = " + ConversionHelper.ToString(printContext.InfodruckHandle);

            mandant.MainDevice.GenericConnection.ExecuteNonQuery("UPDATE tKHKPrintMain SET ConnID = " + ConversionHelper.ToString(printContext.ConnectionID) + clause);
            mandant.MainDevice.GenericConnection.ExecuteNonQuery("UPDATE tPSDPrintTeilnehmer SET ConnID = " + ConversionHelper.ToString(printContext.ConnectionID) + clause);
        }
    }
}
