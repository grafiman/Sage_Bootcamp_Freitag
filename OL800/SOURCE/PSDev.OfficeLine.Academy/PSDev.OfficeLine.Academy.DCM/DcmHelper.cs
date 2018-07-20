using PSDev.OfficeLine.Academy.DataAccess;
using Sagede.Core.Data;
using Sagede.Core.Tools;
using Sagede.OfficeLine.Wawi.BelegEngine;
using Sagede.OfficeLine.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sagede.Shared.RealTimeData.Common.Utilities;

namespace PSDev.OfficeLine.Academy.DCM
{
    public static class DcmHelper
    {
        public const string RelationSeminarbuchungen = "SeminarbuchungCollection";

        private const string UuidTag = "$uuid";
        private const string IsDirtyTag = "IsDirty";
        private const string IsSelectedTag = "IsSelected";
        private const string IsDeletedTag = "IsDeleted";
        private const string IsTemporarySavedTag = "IsTemporarySaved";

        public static DataContainerSet BelegPositionToContainer(this BelegPosition bo)
        {
            var datacontainerSet = new DataContainerSet();
            Seminarbuchungen buchungen;

            if (bo.DCMProperties.ExistParameter(RelationSeminarbuchungen))
            {
                buchungen = (Seminarbuchungen)bo.DCMProperties.ObjectValues[RelationSeminarbuchungen];

                buchungen.ForEach(b =>
                {
                    var dto = new DataContainer();
                    //KeyValue und UuidValue des DTOs müssen zwingend gefüllt werden.
                    //var keys = new string[] { b.BuchungID.ToString() };
                    //dto.KeyValue = EncodeKeys(keys);
                    dto.KeyValue = b.BuchungID.ToString().Base64Encode();
                    if (b.Timestamp != null)
                    {
                        dto.VersionStamp = b.Timestamp.ToString().Base64Encode();
                    }
                    else
                    {
                        dto.VersionStamp = string.Empty;
                    }
                    ClientInfosToContainer(b.Bag, dto);

                    dto.Fill("BuchungID", b.BuchungID);
                    dto.Fill("Mandant", b.Mandant);
                    dto.Fill("SeminarterminID", b.SeminarterminID);
                    dto.Fill("BelID", b.BelID);
                    dto.Fill("BelPosID", b.BelPosID);
                    dto.Fill("VorPosID", b.VorPosID);
                    dto.Fill("Adresse", b.Adresse);
                    dto.Fill("Konto", b.Konto);
                    dto.Fill("KontoMatchcode", b.KontoMatchcode);
                    dto.Fill("Ansprechpartnernummer", b.Ansprechpartnernummer);
                    dto.Fill("AnsprechpartnerVorname", b.AnsprechpartnerVorname);
                    dto.Fill("AnsprechpartnerNachname", b.AnsprechpartnerNachname);
                    dto.Fill("AnsprechpartnerEmail", b.AnsprechpartnerEmail);
                    dto.Fill("EmailBestaetigungGesendet", b.EmailBestaetigungGesendet);
                    datacontainerSet.Add(dto);
                });

            }


            return datacontainerSet;


        }

        public static void FromDataContainer(this Seminarbuchungen item, DataContainer position)
        {
            if (position.ContainsChild(RelationSeminarbuchungen))
            {
                var buchungen = (DataContainerSet)position.GetChild<DataContainerSet>(RelationSeminarbuchungen);
                foreach (DataContainer dto in buchungen)
                {
                    var bo = new Seminarbuchung();
                    //KeyValue und UuidValue müssen zwingend im BO zwischengespeichert werden.
                    //var keys = DecodeKeys(dto.KeyValue);
                    //bo.BuchungID = Conversion.ToInt32(keys[0]);
                    ContainerToClientInfos(bo.Bag, dto);

                    if (dto.ContainsField("BuchungID"))
                    {
                        bo.BuchungID = Conversion.ToInt32(dto["BuchungID"]);
                    }
                    if (dto.ContainsField("Mandant"))
                    {
                        bo.Mandant = Conversion.ToInt16(dto["Mandant"]);
                    }
                    if (dto.ContainsField("SeminarterminID"))
                    {
                        bo.SeminarterminID = Conversion.ToString(dto["SeminarterminID"], false);
                    }
                    if (dto.ContainsField("BelID"))
                    {
                        bo.BelID = Conversion.ToInt32(dto["BelID"]);
                    }
                    if (dto.ContainsField("BelPosID"))
                    {
                        bo.BelPosID = Conversion.ToInt32(dto["BelPosID"]);
                    }
                    if (dto.ContainsField("VorPosID"))
                    {
                        bo.VorPosID = Conversion.ToInt32(dto["VorPosID"]);
                    }
                    if (dto.ContainsField("Adresse"))
                    {
                        bo.Adresse = Conversion.ToInt32(dto["Adresse"]);
                    }
                    if (dto.ContainsField("Konto"))
                    {
                        bo.Konto = Conversion.ToString(dto["Konto"], false);
                    }
                    if (dto.ContainsField("KontoMatchcode"))
                    {
                        bo.KontoMatchcode = Conversion.ToString(dto["KontoMatchcode"], false);
                    }
                    if (dto.ContainsField("Ansprechpartnernummer"))
                    {
                        bo.Ansprechpartnernummer = Conversion.ToInt32(dto["Ansprechpartnernummer"]);
                    }
                    if (dto.ContainsField("AnsprechpartnerVorname"))
                    {
                        bo.AnsprechpartnerVorname = Conversion.ToString(dto["AnsprechpartnerVorname"], false);
                    }
                    if (dto.ContainsField("AnsprechpartnerNachname"))
                    {
                        bo.AnsprechpartnerNachname = Conversion.ToString(dto["AnsprechpartnerNachname"], false);
                    }
                    if (dto.ContainsField("AnsprechpartnerEmail"))
                    {
                        bo.AnsprechpartnerEmail = Conversion.ToString(dto["AnsprechpartnerEmail"], false);
                    }
                    if (dto.ContainsField("EmailBestaetigungGesendet"))
                    {
                        bo.EmailBestaetigungGesendet = Conversion.ToBoolean(dto["EmailBestaetigungGesendet"]);
                    }

                    item.Add(bo);

                }


            }


        }

        private static void ClientInfosToContainer(ParameterBag bag, DataContainer dto)
        {
            if (bag == null)
            {
                bag = new ParameterBag();
            }
            if (bag.ExistParameter(UuidTag) && !string.IsNullOrEmpty(bag.StringValues[UuidTag]))
            {
                dto.UuidValue = bag.StringValues[UuidTag];
            }
            else
            {
                dto.UuidValue = Guid.NewGuid().ToString();
            }
            if (bag.ExistParameter(IsDirtyTag))
            {
                dto[IsDirtyTag] = Convert.ToBoolean(bag.ShortValues[IsDirtyTag]);
            }
            else
            {
                dto[IsDirtyTag] = false;
            }
            if (bag.ExistParameter(IsTemporarySavedTag))
            {
                dto[IsTemporarySavedTag] = Convert.ToBoolean(bag.ShortValues[IsTemporarySavedTag]);
            }
            else
            {
                dto[IsTemporarySavedTag] = false;
            }
            if (bag.ExistParameter(IsSelectedTag))
            {
                dto[IsSelectedTag] = Convert.ToBoolean(bag.ShortValues[IsSelectedTag]);
            }
            else
            {
                dto[IsSelectedTag] = false;
            }
            if (bag.ExistParameter(IsDeletedTag))
            {
                bool isDeleted = Convert.ToBoolean(bag.ShortValues[IsDeletedTag]);
                if (isDeleted)
                    dto.State = DataContainerState.Deleted;
            }
        }

        private static void ContainerToClientInfos(ParameterBag bag, DataContainer dto)
        {
            if (bag == null)
            {
                bag = new ParameterBag();
            }
            bag.StringValues[UuidTag] = dto.UuidValue;

            bool isDirty = false;
            if (dto.ContainsField(IsDirtyTag))
                isDirty = Conversion.ToBoolean(dto[IsDirtyTag]);
            bag.ShortValues[IsDirtyTag] = Convert.ToInt16(isDirty);

            bool isTemporarySaved = false;
            if (dto.ContainsField(IsTemporarySavedTag))
                isTemporarySaved = Conversion.ToBoolean(dto[IsTemporarySavedTag]);
            bag.ShortValues[IsTemporarySavedTag] = Convert.ToInt16(isTemporarySaved);

            bool isSelected = false;
            if (dto.ContainsField(IsSelectedTag))
                isSelected = Conversion.ToBoolean(dto[IsSelectedTag]);
            bag.ShortValues[IsSelectedTag] = Convert.ToInt16(isSelected);

            bool isDeleted = (dto.State == DataContainerState.Deleted);
            bag.ShortValues[IsDeletedTag] = Convert.ToInt16(isDeleted);
        }


    }
}
