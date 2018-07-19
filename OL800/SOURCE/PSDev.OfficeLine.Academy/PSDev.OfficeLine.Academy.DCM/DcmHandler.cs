using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSDev.OfficeLine.Academy.BusinessLogic;
using PSDev.OfficeLine.Academy.DataAccess;
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
                    case DcmDefinitionManager.DcmListId.VKBelegLoad:

                        var vkBelegLoadContext = (Sagede.OfficeLine.Wawi.BelegEngine.DcmContextBeleg)context;
                        var manager = new SeminarbuchungManager(vkBelegLoadContext.Mandant);

                        vkBelegLoadContext.Beleg.Positionen.Where(p => p.Positionstyp == Sagede.OfficeLine.Wawi.Tools.Positionstyp.Artikel).ToList().ForEach(p =>
                        {
                            var buchungen = manager.GetBuchungen(p.VorgangspositionsHandle);
                            if (buchungen.Count > 0)
                            {
                                p.DCMProperties.ObjectValues["Seminarbuchungen"] = buchungen;
                            }
                        });

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
    }
}
