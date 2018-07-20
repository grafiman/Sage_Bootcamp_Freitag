using Sagede.Core.Tools;
using Sagede.Shared.SData.Client;
using Sagede.Shared.Web;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsSDataClient
{
    public partial class FormSDataClient : Form
    {
        private SDataClient _client;
        private SDataContract _contract;

        private List<KeyValuePair<string, string>> _headers;

        public FormSDataClient()
        {
            InitializeComponent();
        }

        private void FormSDataClient_Load(object sender, EventArgs e)
        {
            try
            {
                _client = new Sagede.Shared.SData.Client.SDataClient(SDataClientConfiguration.CreateWithBasicAuthentication("sage", string.Empty));
                _client.Configuration.ContentTypeNegotiation.Set(new[] { MediaType.JSON, MediaType.BinaryContainer, MediaType.BSON, MediaType.Atom });
                _contract = new SDataContract(_client, new Uri(string.Format(Properties.Settings.Default.SDataEndpoint,
                    Properties.Settings.Default.Contract, Properties.Settings.Default.Datenbank, Properties.Settings.Default.Mandant)));
                //https://HAL9000-VM01:5493/sdata/ol/MasterData/OLDemoReweAbfD;123

                _headers = new List<KeyValuePair<string, string>>();
                _headers.Add(new KeyValuePair<string, string>("X-ApplicationId", "AddOn"));
                _headers.Add(new KeyValuePair<string, string>("X-ContextToken", "Application"));
                _headers.Add(new KeyValuePair<string, string>("X-MachineName", Properties.Settings.Default.Rechnername));
                _headers.Add(new KeyValuePair<string, string>("X-PartContext", Properties.Settings.Default.Partname));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            try
            {
                var buchung = _contract.Resource("strSeminarbuchung.100096740.Academy").Read().
                    ByKey(this.textBoxBuchungsID.Text.Base64Encode()).
                    WithHeaders(_headers).IncludeChildren().AsContainerResponse<Seminarbuchung>().Data;

                this.textBoxEmail.Text = buchung.AnsprechpartnerEmail;
                this.textBoxKonto.Text = buchung.Konto;
                this.textBoxNachname.Text = buchung.AnsprechpartnerNachname;
                this.textVorname.Text = buchung.AnsprechpartnerVorname;
                this.textBoxBuchungsID.Text = buchung.BuchungsID.ToString();
                this.textBoxReferenz.Text = string.Format("{0}/{1}/{2}", buchung.BuchungsID, buchung.BelID, buchung.BelPosID);
                this.textBoxSeminar.Text = buchung.SeminarterminId;
                buchung.UuidValue = System.Guid.NewGuid().ToString();
                buchung.VersionStamp = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            try
            {
                var buchung = new Seminarbuchung()
                {
                    AnsprechpartnerEmail = this.textBoxEmail.Text,
                    AnsprechpartnerNachname = this.textBoxNachname.Text,
                    AnsprechpartnerVorname = this.textBoxNachname.Text,
                    Konto = this.textBoxKonto.Text,
                    SeminarterminId = this.textBoxSeminar.Text,
                    BuchungsID = ConversionHelper.ToInt32(this.textBoxBuchungsID.Text)
                };

                var result = _contract.Resource("strSeminarbuchung.100096740.Academy").
                    CreateAsDataContainer<Seminarbuchung>(string.Empty, buchung, true, ToDictionary(_headers, x => x.Key, x => x.Value)).Data;

                this.textBoxBuchungsID.Text = result.BuchungsID.ToString();
                this.textBoxReferenz.Text = string.Format("{0}/{1}/{2}", result.BuchungsID, result.BelID, result.BelPosID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Hilfsfunktion, um eine List von KeyValue Paaren in ein Dictionary zu wandeln."/>,
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            if (elementSelector == null)
            {
                throw new ArgumentNullException("elementSelector");
            }

            var dictionary = new Dictionary<TKey, TElement>();
            foreach (TSource current in source)
            {
                dictionary.Add(keySelector(current), elementSelector(current));
            }
            return dictionary;
        }
    }
}