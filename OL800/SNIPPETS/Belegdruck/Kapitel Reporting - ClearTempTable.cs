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