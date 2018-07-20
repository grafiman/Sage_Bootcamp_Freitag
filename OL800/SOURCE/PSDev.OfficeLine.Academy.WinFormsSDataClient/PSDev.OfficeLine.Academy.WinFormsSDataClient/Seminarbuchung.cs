using Sagede.Core.Data;

namespace WinFormsSDataClient
{
    public class Seminarbuchung : DataContainer
    {
        public Seminarbuchung()
        { }

        public Seminarbuchung(IDataContainerElement dc) : base(dc)
        {
        }

        /// <summary>
        /// BuchungsID
        /// </summary>
        public int BuchungsID
        {
            get { return GetValue<int>("BuchungsID"); }
            set { base["BuchungsID"] = value; }
        }

        /// <summary>
        /// BelID
        /// </summary>
        public int BelID
        {
            get { return GetValue<int>("BelID"); }
            set { base["BelID"] = value; }
        }

        /// <summary>
        /// BelPosID
        /// </summary>
        public int BelPosID
        {
            get { return GetValue<int>("BelPosID"); }
            set { base["BelPosID"] = value; }
        }

        /// <summary>
        /// AnsprechpartnerNachname
        /// </summary>
        public string AnsprechpartnerNachname
        {
            get { return GetValue<string>("AnsprechpartnerNachname"); }
            set { base["AnsprechpartnerNachname"] = value; }
        }

        /// <summary>
        /// AnsprechpartnerVorname
        /// </summary>
        public string AnsprechpartnerVorname
        {
            get { return GetValue<string>("AnsprechpartnerVorname"); }
            set { base["AnsprechpartnerVorname"] = value; }
        }

        /// <summary>
        /// AnsprechpartnerEmail
        /// </summary>
        public string AnsprechpartnerEmail
        {
            get { return GetValue<string>("AnsprechpartnerEmail"); }
            set { base["AnsprechpartnerEmail"] = value; }
        }

        /// <summary>
        /// Konto
        /// </summary>
        public string Konto
        {
            get { return GetValue<string>("Konto"); }
            set { base["Konto"] = value; }
        }

        /// <summary>
        /// SeminarterminId
        /// </summary>
        public string SeminarterminId
        {
            get { return GetValue<string>("SeminarterminId"); }
            set { base["SeminarterminId"] = value; }
        }
    }
}