        private void CompleteTempTable(IDcmContext context)
        {
            var printContext = (Sagede.OfficeLine.Wawi.PrintEngine.DcmContextBelegdruck)context;
            var mandant = printContext.Mandant;
            String clause;

            clause = " WHERE ConnID = 0 AND InfodruckId = " + ConversionHelper.ToString(printContext.InfodruckHandle);

            mandant.MainDevice.GenericConnection.ExecuteNonQuery("UPDATE tKHKPrintMain SET ConnID = " + ConversionHelper.ToString(printContext.ConnectionID) + clause);
            mandant.MainDevice.GenericConnection.ExecuteNonQuery("UPDATE tPSDPrintTeilnehmer SET ConnID = " + ConversionHelper.ToString(printContext.ConnectionID) + clause);
        }