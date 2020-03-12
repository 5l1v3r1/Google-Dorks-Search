using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//Eklenenler
using System.Threading;
using System.Net;
using HtmlAgilityPack;
using System.IO;

namespace Google_Dorks_Search
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string hedefSite = null;
        string kodlamaDili = null;
        public string veri;
        string html = null;
        int dorkSure = 0;
        int sayfa = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
            //Thread Çalıştırma
            CheckForIllegalCrossThreadCalls = false;

            //Form Boyutu
            //this.Size = new Size(668, 580);

            //Captcha Size
            this.Size = new Size(1322, 580);
        }

        #region Veri Ayıklama Fonksiyonu
        void veriAyiklama(string kaynakKod, string ilkVeri, int ilkVeriKS, string sonVeri)
        {
            try
            {
                string gelen = kaynakKod;
                int titleIndexBaslangici = gelen.IndexOf(ilkVeri) + ilkVeriKS;
                int titleIndexBitisi = gelen.Substring(titleIndexBaslangici).IndexOf(sonVeri);
                veri = gelen.Substring(titleIndexBaslangici, titleIndexBitisi);
            }
            catch //(Exception ex)
            {
                //MessageBox.Show("Hata: " + ex.Message, "Hata;", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Taramayı Başlat
        private void button1_Click(object sender, EventArgs e)
        {
            hedefSite = textBox1.Text;
            kodlamaDili = comboBox1.Text;
            if (hedefSite == "" || hedefSite.Length < 2 || textBox9.Text == "")
            {
                MessageBox.Show("Taramayı başlatabilmek için hedef site veya özel dork girmelisiniz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (listBox1.Items.Count > 0 || timer1.Enabled == true) 
                {
                    DialogResult soru = MessageBox.Show("Daha önceki tarama kayıtları mevcut. Yeni tarama başlatmak istiyor musunuz?","Bilgi", MessageBoxButtons.YesNo,MessageBoxIcon.Information);
                    if (soru == DialogResult.Yes) 
                    {
                        textBox1.Clear();
                        textBox2.Clear();
                        textBox3.Clear();
                        textBox4.Clear();
                        listBox1.Items.Clear();
                        comboBox1.Text = "";
                    }
                }
                else
                {
                    if (comboBox2.Text == "Sabit Dork Tarama")
                    {
                        //Hedef Düzenleme
                        hedefSite = hedefSite.Replace("http://", "");
                        hedefSite = hedefSite.Replace("https://", "");
                        hedefSite = hedefSite.Replace("www.", "");
                        hedefSite = hedefSite.Replace("/", "");

                        //Dork
                        textBox2.Text = "site:" + hedefSite + " filetype:" + kodlamaDili + " inurl:." + kodlamaDili + "?";

                        //Buton Aktifliği
                        button1.Enabled = false;
                        button2.Enabled = true;
                        textBox1.Enabled = false;
                        comboBox1.Enabled = false;
                        comboBox2.Enabled = false;

                        //Bilgi Mesajı
                        StatusLabel1.ForeColor = Color.DarkBlue;
                        StatusLabel1.Text = "Tarama işlemi başladı.";

                        //İşlemler
                        sayfa = 0;
                        timer1.Enabled = true;
                    }
                    else if (comboBox2.Text == "Özel Dork Tarama")
                    {
                        //Buton Aktifliği
                        button1.Enabled = false;
                        button2.Enabled = true;
                        textBox1.Enabled = false;
                        comboBox1.Enabled = false;
                        comboBox2.Enabled = false;

                        //Bilgi Mesajı
                        StatusLabel1.ForeColor = Color.DarkBlue;
                        StatusLabel1.Text = "Tarama işlemi başladı.";

                        //İşlemler
                        sayfa = 0;
                        timer1.Enabled = true;
                    }
                    else if (comboBox2.Text == "Tarama türü seçiniz..")
                    {
                        //Bilgi Mesajı
                        StatusLabel1.ForeColor = Color.DarkRed;
                        StatusLabel1.Text = "Tarama türünü seçmeden tarama işlemi başlatamazsınız..";
                        MessageBox.Show("Tarama türünü seçmeden tarama işlemi başlatamazsınız..", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //Bitiş
                        //Buton Aktifliği
                        button2.Enabled = false;
                        button1.Enabled = true;
                        textBox1.Enabled = true;
                        comboBox1.Enabled = true;
                        comboBox2.Enabled = true;
                    }
                }
            }
        }
        #endregion

        #region Taramayı Durdur
        private void button2_Click(object sender, EventArgs e)
        {
            //Tarama Durdur
            timer1.Enabled = false;
            sayfa = 0;
            dorkSure = 0;
            webBrowser1.Stop();

            //Buton Aktifliğ
            button2.Enabled = false;
            button1.Enabled = true;
            textBox1.Enabled = true;
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;

            //Bilgi Mesajı
            StatusLabel1.ForeColor = Color.DarkRed;
            StatusLabel1.Text = "Tarama işlemi durduruldu.";
        }
        #endregion

        #region Listbox Seçili Link
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox3.Text = listBox1.Text;
        }
        #endregion

        #region Tarama Türleri Seçme
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "Sabit Dork Tarama")
            {
                //Sabit Dork Tarama
                panel1.Visible = true;
                panel1.Location = new Point(9, 41);
                panel1.Size = new Size(447, 41);
                panel2.Visible = false;
                panel1.Enabled = true;
            }
            else if (comboBox2.Text == "Özel Dork Tarama")
            {
                //Özel Dork Tarama
                panel1.Visible = false;
                panel2.Visible = true;
                panel2.Location = new Point(9, 41);
                panel2.Size = new Size(447, 41);
                panel2.Enabled = true;
            }
            else if (comboBox2.Text == "Tarama türü seçiniz..")
            {
                //engelle
                panel2.Enabled = false;
                panel1.Enabled = false;
            }
        }
        #endregion

        #region Tarama türü farklı giriş engelle
        private void comboBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = Char.IsLetterOrDigit(e.KeyChar) || Char.IsSymbol(e.KeyChar) || Char.IsPunctuation(e.KeyChar) || Char.IsWhiteSpace(e.KeyChar) || Char.IsControl(e.KeyChar) || Char.IsNumber(e.KeyChar);
        }
        #endregion

        #region Sonuçları Kaydet
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog kaydet = new SaveFileDialog();
            kaydet.Title = "Kaydedilecek yeri seçiniz..";
            kaydet.Filter = "Metin Belgesi|*.txt";
            DialogResult soru = kaydet.ShowDialog();
            if (soru == DialogResult.OK)
            {
                //Dosyayı açmaya çalış yoksa catch kısmına geç
                try
                {
                    StreamReader Dosya = File.OpenText(kaydet.FileName);
                    Dosya.Close();
                    File.Delete(kaydet.FileName);
                    try
                    {
                        //Dosyayı appendText ile yazmak için açtık
                        StreamWriter dosyaAc = File.AppendText(kaydet.FileName);
                        // Dosya.WriteLine ile dosyaya verileri ekledik.
                        dosyaAc.WriteLine("## " + this.Text + " ##");
                        dosyaAc.WriteLine("Kayıt Tarihi= " + DateTime.Now);
                        dosyaAc.WriteLine("Toplam Site = " + listBox1.Items.Count.ToString() + "\n");

                        foreach (var item in listBox1.Items)
                        {
                            dosyaAc.WriteLine(item);
                        }
                        // Dosya yı kapattık.
                        dosyaAc.Close();
                        MessageBox.Show("Sonuçlar başarıyla kaydedildi.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch { }
                }
                catch
                {
                    try
                    {
                        //Dosyayı appendText ile yazmak için açtık
                        StreamWriter dosyaAc = File.AppendText(kaydet.FileName);
                        // Dosya.WriteLine ile dosyaya verileri ekledik.
                        dosyaAc.WriteLine("## " + this.Text + " ##");
                        dosyaAc.WriteLine("Kayıt Tarihi= " + DateTime.Now);
                        dosyaAc.WriteLine("Toplam Site = " + listBox1.Items.Count.ToString() + "\n");

                        foreach (var item in listBox1.Items)
                        {
                            dosyaAc.WriteLine(item);
                        }
                        // Dosya yı kapattık.
                        dosyaAc.Close();
                        MessageBox.Show("Sonuçlar başarıyla kaydedildi.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch { }
                }
            }
            else
            {
                MessageBox.Show("Sonuçları kaydetmediniz.", "Bilgi;", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        #endregion

        #region Programı Kapat
        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult soru = MessageBox.Show("Program kapatılsın mı?","İşlem;",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (soru == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            dorkSure = dorkSure + 1;
            label9.Text = dorkSure.ToString();
            if (dorkSure == int.Parse(textBox8.Text))
            {
                if (comboBox2.Text == "Sabit Dork Tarama")
                {
                    sayfa = sayfa + 10;
                    webBrowser1.Navigate("https://www.google.com.tr/search?num=100&q=site:" + hedefSite + "+filetype:" + kodlamaDili + "+inurl:." + kodlamaDili + "?" + "&start=" + sayfa);
                }
                else if (comboBox2.Text == "Özel Dork Tarama")
                {
                    sayfa = sayfa + 10;
                    webBrowser1.Navigate("https://www.google.com.tr/search?num=100&q=" + textBox9.Text + "&start=" + sayfa);
                }
            }
        }

        #region WebBrowser Tamamlandığında Urlleri Çek
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            html = webBrowser1.Document.Body.InnerHtml.ToString();
            richTextBox1.Text = html;
            if (html.IndexOf("CaptchaRedirect") != -1)
            {
                //Captcha Size
                this.Size = new Size(1322, 580);
            }
            else if (html.IndexOf("ile ilgili hiçbir arama sonucu mevcut değil") != -1)
            {
                //Tarama Durdur
                timer1.Enabled = false;
                sayfa = 0;
                dorkSure = 0;
                webBrowser1.Stop();

                //Buton Aktifliğ
                button2.Enabled = false;
                button1.Enabled = true;
                textBox1.Enabled = true;
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;

                //Bilgi Mesajı
                StatusLabel1.ForeColor = Color.DarkRed;
                StatusLabel1.Text = "Hiçbir sonuç bulunamadı.";
                MessageBox.Show("Hiçbir sonuç bulunamadı.","Bilgi",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            else
            {
                #region toplam sonuç
                /*string sonuc = null;

                HtmlAgilityPack.HtmlDocument dokuman2 = new HtmlAgilityPack.HtmlDocument();
                dokuman2.LoadHtml(html);
                HtmlNodeCollection XPath2 = dokuman2.DocumentNode.SelectNodes("//div[@id='resultStats']");
                foreach (var veri2 in XPath2)
                {
                    veriAyiklama(veri2.InnerText, "/", 1, " ");
                    sonuc = veri;
                }

                MessageBox.Show(sonuc);*/
                #endregion
               
                try
                {
                    if (comboBox2.Text == "Sabit Dork Tarama")
                    {
                        HtmlAgilityPack.HtmlDocument dokuman = new HtmlAgilityPack.HtmlDocument();
                        dokuman.LoadHtml(html);
                        HtmlNodeCollection XPath = dokuman.DocumentNode.SelectNodes("//h3[@class='r']");
                        foreach (var veri2 in XPath)
                        {
                            //Veri Ayıklama
                            veriAyiklama(veri2.InnerHtml, "href=", 5, ">");
                            veri = veri.Replace("%3F", "?");
                            veri = veri.Replace("%3D", "=");
                            veri = veri.Replace(textBox4.Text, "");
                            veri = veri.Replace("&amp;", "&");
                            if (listBox1.Items.Contains(veri) == false)
                            {
                                listBox1.Items.Add(veri);
                            }
                        }
                        //Toplam url
                        textBox5.Text = listBox1.Items.Count.ToString();

                        dorkSure = 0;

                        //Form Boyutu
                        //this.Size = new Size(668, 580);

                        /*//Bitiş
                        //Buton Aktifliği
                        button2.Enabled = false;
                        button1.Enabled = true;
                        textBox1.Enabled = true;
                        comboBox1.Enabled = true;

                        //Bilgi Mesajı
                        StatusLabel1.ForeColor = Color.DarkGreen;
                        StatusLabel1.Text = "Tarama işlemi tamamlandı.";*/
                    }
                }
                catch { }
            }
        }
        #endregion
    }
}
