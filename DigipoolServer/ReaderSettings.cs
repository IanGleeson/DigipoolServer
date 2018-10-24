using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DigipoolServer
{
    public partial class ReaderSettings : Form
    {
        private const string FILENAME = "Reader Settings.xml";
        XDocument doc;

        public ReaderSettings()
        {
            InitializeComponent();

            try
            {
                doc = XDocument.Load(FILENAME);
                reader_Address.Text = doc.Root.Element("ReaderAddress").Value;
                tx_Power_In_Dbm.Text = doc.Root.Element("TxPowerInDbm").Value;
            } catch (FileNotFoundException)
            {
                doc = new XDocument(
                    new XElement("ReaderSettings",
                        new XElement("ReaderAddress"),
                        new XElement("TxPowerInDbm")
                    )
                );
            }
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            var readerAddress = reader_Address.Text;
            var txPowerInDbm = tx_Power_In_Dbm.Text;
            
            doc.Root.Element("ReaderAddress").Value = readerAddress;
            if (string.IsNullOrWhiteSpace(txPowerInDbm))
            {
                doc.Root.Element("TxPowerInDbm").Value = "20";
            } else
            {
                doc.Root.Element("TxPowerInDbm").Value = txPowerInDbm;
            }
            
            doc.Save(FILENAME);
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txPowerInDbm_TextChanged(object sender, EventArgs e)
        {

        }

        private void readerAddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
