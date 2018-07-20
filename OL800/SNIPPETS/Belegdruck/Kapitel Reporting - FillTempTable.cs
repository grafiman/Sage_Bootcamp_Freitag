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