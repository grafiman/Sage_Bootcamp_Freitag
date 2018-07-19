using System;
using Sagede.Core.Data;
using Sagede.Core.Expressions;
using Sagede.Core.Tools;
using Sagede.Shared.RealTimeData.Common;
using Sagede.Shared.RealTimeData.Common.Utilities;
using Sagede.OfficeLine.Shared;
using Sagede.OfficeLine.Shared.RealTimeData;
using PSDev.OfficeLine.Academy.BusinessLogic;
using PSDev.OfficeLine.Academy.DataAccess;
//###Imports###

namespace PSDev.OfficeLine.Academy.RealTimeData
{
    public class Seminarbuchung : DataServiceBase
    {
        private const string KeyField1 = "BuchungID";

        private const string UuidTag = "$uuid";
        private const string IsDirtyTag = "IsDirty";
        private const string IsSelectedTag = "IsSelected";
        private const string IsDeletedTag = "IsDeleted";
        private const string IsTemporarySavedTag = "IsTemporarySaved";

        //UuIds werden im Client dazu verwendet, Datensätze eindeutig zu identifizieren.
        //Da beim Aufruf von Daten-Diensten (DataServices) der Datensatz noch nicht gespeichert sein muss
        //und daher noch kein Key exisitert.
        //CRUD-Operationen (Anlegen, Lesen, Aktualisieren und Löschen) werden immer anhand des Keys durchgeführt.
        //Hierbei spielt die UuId keine Rolle.
        //Beim Update sollte der vom Client gelieferte VersionStamp gegen den aktuell in der Datenbank
        //gespeicherten geprüft werden und das Update zurückgewiesen werden, wenn keine Übereinstimmung vorliegt.
        //var dar = new DataActionResult<DataContainer>();
        //dar.Status = DataActionResultStatus.NotFound;
        //dar.Messages.Add(new DataActionResultMessage() { Severity = DataActionResultMessageSeverity.Error, Message = ex.Message });
        #region Client Helper

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

        #endregion

        #region Overrides

        /// <summary>
        /// PrepareMetadata: Set KeyFields, Defaults, FieldLength etc.
        /// </summary>
        /// <param name="rawMetadata"></param>
        /// <remarks></remarks>
        public override void PrepareMetadata(DataObjectBase rawMetadata)
        {
            DataStructure dataStructure = rawMetadata as DataStructure;
            if (dataStructure != null)
            {
                if (!string.IsNullOrEmpty(KeyField1))
                {
                    DataFieldObjectBase field1 = dataStructure.Fields.TryGetItem(KeyField1);
                    if (field1 != null)
                    {
                        dataStructure.PrimaryKeyField1 = KeyField1;
                    }
                }
            }
        }

        /// <summary>
        /// Get an empty item
        /// </summary>
        /// <param name="request"></param>
        /// <returns>DataActionResult</returns>
        /// <remarks></remarks>
        public override DataActionResult<DataContainer> GetTemplate(DataActionRequest request)
        {
            var result = new DataContainer();
            var bo = new PSDev.OfficeLine.Academy.DataAccess.Seminarbuchung();

            //UUIDs müssen erzeugt werden
            FillDto(result, bo);
            result.KeyValue = string.Empty;
            result.VersionStamp = string.Empty;

            return DataActionResult.Succeeded(result);
        }

        /// <summary>
        /// Get an item by key
        /// </summary>
        /// <param name="request"></param>
        /// <returns>DataActionResult</returns>
        /// <remarks></remarks>
        public override DataActionResult<DataContainer> GetItem(DataActionRequest request)
        {
            var keys = DecodeKeys(request.Key);
            var result = new DataContainer();
            var bo = new PSDev.OfficeLine.Academy.DataAccess.Seminarbuchung();
            var manager = new SeminarbuchungManager(Mandant);

            try
            {
                bo = manager.GetBuchung(Convert.ToInt32(keys[0]));
                //UUIDs müssen erzeugt oder evtl. gespeicherte verwendet werden
                FillDto(result, bo);
                return DataActionResult.Succeeded(result);
            }
            catch (RecordNotFoundException ex)
            {
                return DataActionResult.NotFound<DataContainer>();
            }
            catch (Exception ex)
            {
                return DataActionResult.ServerError<DataContainer>(ex);
            }
        }

        /// <summary>
        /// Get a list of items by where expression
        /// </summary>
        /// <param name="request"></param>
        /// <returns>DataActionResult</returns>
        /// <remarks>Only available resource requests</remarks>
        public override DataActionResult<DataContainerSet> GetList(DataActionRequest request)
        {
            var result = new DataContainerSet();

            //for (Int32 i = 0; i <= 10; i++)
            //{
            //    var row = new DataContainer();
            //
            //    var bo = new PSDev.OfficeLine.Academy.DataAccess.Seminarbuchung(Mandant);
            //    bo.Load(i);
            //
            //    //UUIDs müssen erzeugt oder evtl. gespeicherte verwendet werden
            //    FillDto(row, bo);
            //
            //    result.Add(row);
            //}

            return DataActionResult.Succeeded(result);
        }

        /// <summary>
        /// Create a new item
        /// </summary>
        /// <param name="request"></param>
        /// <returns>DataActionResult</returns>
        /// <remarks></remarks>
        public override DataActionResult<DataContainer> Create(DataActionRequest<DataContainer> request)
        {
            var bo = new PSDev.OfficeLine.Academy.DataAccess.Seminarbuchung();
            var manager = new SeminarbuchungManager(Mandant);

            try
            {
                //UUIDs werden vom Client mitgeschickt und müssen während des Round-Trips erhalten bleiben
                //Key kommt aus dem DataContainer oder wird vor dem Speichern erzeugt.
                FillBo(bo, request.Data);
                bo = manager.CreateOrUpdateBuchungsbeleg(bo);
                FillDto(request.Data, bo);

                return DataActionResult.Succeeded(request.Data);
            }
            catch (BuchungValidationException ex)
            {
                return DataActionResult.ValidationFailed<DataContainer>(ex.Message, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                return DataActionResult.ServerError<DataContainer>(ex);
            }
        }

        /// <summary>
        /// Update an existing item
        /// </summary>
        /// <param name="request"></param>
        /// <returns>DataActionResult</returns>
        /// <remarks></remarks>
        public override DataActionResult<DataContainer> Update(DataActionRequest<DataContainer> request)
        {
            var keys = DecodeKeys(request.Data.KeyValue);
            var bo = new PSDev.OfficeLine.Academy.DataAccess.Seminarbuchung();
            var manager = new SeminarbuchungManager(Mandant);

            try
            {
                bo = manager.GetBuchung(Convert.ToInt32(keys[0]));

                //Prüfen, ob der Datensatz bereits durch einen anderen Benutzer geändert wurde
                //und ggf. eine Konflikt-Meldung mit den aktuellen Daten zurückgeben.
                if (bo.Timestamp.ToString().Base64Encode() != request.VersionStamp)
                {
                    return DataActionResult.Concurrency(GetItem(request).Data);
                }

                //UUIDs werden vom Client mitgeschickt und müssen während des Round-Trips erhalten bleiben
                //Daten von Unterobjekten müssen zusammengeführt werden.
                FillBo(bo, request.Data);
                bo = manager.CreateOrUpdateBuchungsbeleg(bo);

                FillDto(request.Data, bo);
                return DataActionResult.Succeeded(request.Data);
            }
            catch (BuchungValidationException ex)
            {
                return DataActionResult.Forbidden<DataContainer>(ex.Message);
            }
            catch (RecordNotFoundException)
            {
                return DataActionResult.NotFound<DataContainer>();
            }
            catch (Exception ex)
            {
                return DataActionResult.ServerError<DataContainer>(ex);
            }

        }

        /// <summary>
        /// Delete an existing item
        /// </summary>
        /// <param name="request"></param>
        /// <returns>DataActionResult</returns>
        /// <remarks></remarks>
        public override DataActionResult Delete(DataActionRequest request)
        {
            throw new NotImplementedException("Delete");
        }

        /// <summary>
        /// Execute Dataservice
        /// </summary>
        /// <param name="request"></param>
        /// <returns>DataServiceExecuteResponse</returns>
        /// <remarks></remarks>
        public override DataServiceExecuteResponse Execute(DataServiceExecuteRequest request)
        {
            var bo = new PSDev.OfficeLine.Academy.DataAccess.Seminarbuchung();
            FillBo(bo, request.Data);

            switch (request.MethodName)
            {
                //Verfügbare Methoden:


            }

            //UUIDs werden vom Client mitgeschickt und müssen während des Round-Trips erhalten bleiben
            FillDto(request.Data, bo);

            return DataServiceExecuteResponse.SuccessResponse(request.Data);
        }

        #endregion

        #region Fill Data and Buiness Objects

        /// <summary>
        /// Fill data transfer object by business object
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="bo"></param>
        /// <remarks></remarks>
        private static void FillDto(DataContainer dto, PSDev.OfficeLine.Academy.DataAccess.Seminarbuchung bo)
        {
            FillDtoSeminarbuchung(dto, bo);
        }

        /// <summary>
        /// Fill business object by data transfer object
        /// </summary>
        /// <param name="bo"></param>
        /// <param name="dto"></param>
        /// <remarks></remarks>
        private static void FillBo(PSDev.OfficeLine.Academy.DataAccess.Seminarbuchung bo, DataContainer dto)
        {

            FillBoSeminarbuchung(bo, dto);
        }

        private static void FillDtoSeminarbuchung(DataContainer dto, PSDev.OfficeLine.Academy.DataAccess.Seminarbuchung bo)
        {
            //KeyValue und UuidValue des DTOs müssen zwingend gefüllt werden.
            var keys = new string[] { bo.BuchungID.ToString() };
            dto.KeyValue = EncodeKeys(keys);
            if (bo.Timestamp != null)
            {
                dto.VersionStamp = bo.Timestamp.ToString().Base64Encode();
            }
            else
            {
                dto.VersionStamp = string.Empty;
            }
            ClientInfosToContainer(bo.Bag, dto);

            dto.Fill("BuchungID", bo.BuchungID);
            dto.Fill("Mandant", bo.Mandant);
            dto.Fill("SeminarterminID", bo.SeminarterminID);
            dto.Fill("BelID", bo.BelID);
            dto.Fill("BelPosID", bo.BelPosID);
            dto.Fill("VorPosID", bo.VorPosID);
            dto.Fill("Adresse", bo.Adresse);
            dto.Fill("Konto", bo.Konto);
            dto.Fill("KontoMatchcode", bo.KontoMatchcode);
            dto.Fill("Ansprechpartnernummer", bo.Ansprechpartnernummer);
            dto.Fill("AnsprechpartnerVorname", bo.AnsprechpartnerVorname);
            dto.Fill("AnsprechpartnerNachname", bo.AnsprechpartnerNachname);
            dto.Fill("AnsprechpartnerEmail", bo.AnsprechpartnerEmail);
            dto.Fill("EmailBestaetigungGesendet", bo.EmailBestaetigungGesendet);

        }

        private static void FillBoSeminarbuchung(PSDev.OfficeLine.Academy.DataAccess.Seminarbuchung bo, DataContainer dto)
        {
            //KeyValue und UuidValue müssen zwingend im BO zwischengespeichert werden.
            var keys = DecodeKeys(dto.KeyValue);
            bo.BuchungID = Conversion.ToInt32(keys[0]);
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

        }




        #endregion

    }
}