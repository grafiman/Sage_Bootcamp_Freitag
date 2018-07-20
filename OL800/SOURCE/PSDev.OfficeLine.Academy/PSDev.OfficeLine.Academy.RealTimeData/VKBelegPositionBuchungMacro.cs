using PSDev.OfficeLine.Academy.DataAccess;
using Sagede.Core.Tools;
using Sagede.OfficeLine.Shared.RealTimeData.MacroProcess;
using Sagede.Shared.RealTimeData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSDev.OfficeLine.Academy.RealTimeData
{
    public class VKBelegPositionBuchungMacro : MacroProcessBase
    {
        protected override string Name => "";

        protected override NamedParameters Execute(NamedParameters parameters, ref bool cancel, ref string cancelMessage)
        {
            try
            {
                cancel = false;
                cancelMessage = string.Empty;

                var nummer = ConversionHelper.ToInt32( parameters.GetItemEx("Ansprechpartnernummer").Value);
                var ansprechpartnerItem = SeminarData.GetAnsprechpartner(Mandant, nummer);
                parameters.GetItemEx("AnsprechpartnerVorname").Value = ansprechpartnerItem.Vorname;
                parameters.GetItemEx("AnsprechpartnerNachname").Value = ansprechpartnerItem.Nachname;
                parameters.GetItemEx("AnsprechpartnerEmail").Value = ansprechpartnerItem.EMail;
                return parameters;
            }
            catch (Exception ex)
            {
                cancel = true;
                cancelMessage = ex.Message;
                return parameters;
            }
        }

        protected override void Prepare()
        {
           
        }
    }
}
