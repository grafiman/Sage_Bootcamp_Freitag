using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSDev.OfficeLine.Academy.BusinessLogic;
using PSDev.OfficeLine.Academy.DataAccess;
using Sagede.Core.Data;
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
    }
}
