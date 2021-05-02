using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Data.OleDb;


namespace satıssipuyg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        #region  değişkenler,sınıflar ve methodlar
        //sql bağlantı sınıfı
        baglanti bgl = new baglanti();


        //listler
        List<string> stokbilgisi = new List<string>();
        List<string> firmaunvan = new List<string>();
        List<string> stokmiktarı = new List<string>();
        List<string> stokkodu = new List<string>();
        List<string> stokismi = new List<string>();
        List<string> mailadresleri = new List<string>();


        //değişkenler
        int bizimPcGenislik = 1920;
        int bizimPcYukseklik = 1080;
        float kullanilanPcGenislik = SystemInformation.PrimaryMonitorSize.Width;
        float kullanilanPcYukseklik = SystemInformation.PrimaryMonitorSize.Height;
        int miktar;
        int sırasayısı;
        int secilensatır;
        decimal birimfiyat, tutar;
        decimal toplam;
        bool satıreklemi;
        bool guncelenebilirmi;
        bool mailgonderilsinmi;
        bool satireklenmis;
        bool kontrol;
        public string uygulamaklasoru = Application.StartupPath;
        string faturaturu;
        string kargoodeme;
        string htmlkodu;
        string mail;
        string bosgecilenalanlar;


        //methodlar
        void stokbilgileritemizle()
        {
            satıreklemi = false;
            txtbirimfiyat.Text = "";
            lblstokmiktari.Text = "Stok :";
            lkupstokbilgi.Text = "";
            txtmiktar.Text = "";
            satıreklemi = true;
        }

        void satırekle()
        {
            if (satıreklemi == true)
            {
                if (lkupstokbilgi.Text == "")
                    DevExpress.XtraEditors.XtraMessageBox.Show("Lütfen Stok kodu ve stok ismini doldurduğunuzdan emin sonra tekrar deneyiniz.");
                else
                {
                    sırasayısı++;
                    tablo.Rows.Add(sırasayısı, lkupstokbilgi.Text, txtmiktar.Text, txtbirimfiyat.Text, tutar);

                    if (sırasayısı == 1)
                        toplam = decimal.Parse(tablo.Rows[0].Cells[4].Value.ToString());
                    else
                        toplam += decimal.Parse(tablo.Rows[sırasayısı - 1].Cells[4].Value.ToString());
                    txttoplam.Text = toplam.ToString();

                }
            }
        }

        void stokbilgileridoldur()
        {
            try
            {
                OleDbCommand komut = new OleDbCommand("select * from [stok bilgileri$]", bgl.excelstokbilgizconnection());
                OleDbDataReader dr = komut.ExecuteReader();
                while (dr.Read())
                {
                    stokkodu.Add(dr[0].ToString());
                    stokismi.Add(dr[1].ToString());
                    stokmiktarı.Add(dr[2].ToString());
                }
                dr.Close();
                bgl.excelstokbilgizconnection().Close();

                for (int i = 0; i <= stokkodu.Count - 1; i++)
                {
                    stokbilgisi.Add(stokkodu[i] + " - " + stokismi[i]);
                }

                lkupstokbilgi.Properties.DataSource = stokbilgisi;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Exit();
            }

        }

        void digercomboliste()
        {
            try
            {
                    StreamReader plasiyeradi = new StreamReader(uygulamaklasoru + @"\satış sipariş uygulaması gerekli dosyalar\Plasiyerler.txt");
                    string satırplasiyer = plasiyeradi.ReadLine();
                    while (satırplasiyer != null)
                    {
                        cmbplasiyeradi.Properties.Items.Add(satırplasiyer);
                        satırplasiyer = plasiyeradi.ReadLine();
                    }
                    plasiyeradi.Close();

                    StreamReader dovizturu = new StreamReader(uygulamaklasoru + @"\satış sipariş uygulaması gerekli dosyalar\döviz.txt");
                    string satırdovizturu = dovizturu.ReadLine();
                    while (satırdovizturu != null)
                    {
                        cmbdovizturu.Properties.Items.Add(satırdovizturu);
                        satırdovizturu = dovizturu.ReadLine();
                    }
                    dovizturu.Close();

                    StreamReader teslimatsekli = new StreamReader(uygulamaklasoru + @"\satış sipariş uygulaması gerekli dosyalar\teslimatsekli.txt");
                    string satırteslimatsekli = teslimatsekli.ReadLine();
                    while (satırteslimatsekli != null)
                    {
                        cmbteslimatsekl.Properties.Items.Add(satırteslimatsekli);
                        satırteslimatsekli = teslimatsekli.ReadLine();
                    }
                    teslimatsekli.Close();

                StreamReader vade = new StreamReader(uygulamaklasoru + @"\satış sipariş uygulaması gerekli dosyalar\vade.txt");
                string satırvade = vade.ReadLine();
                while (satırvade != null)
                {
                    Cmbvade.Properties.Items.Add(satırvade);
                    satırvade = vade.ReadLine();
                }
                teslimatsekli.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Exit();
            }
        }

        void herseyitemizle()
        {
            cmbplasiyeradi.Text = "";
            cmbdovizturu.Text = "$";
            cmbteslimatsekl.Text = "";
            Cmbvade.Text = "Ön ödeme";
            txtilgilikisitel.Text = "";
            txtteslimatadrs.Text = "";
            txttoplam.Text = "";
            txtnotlar.Text = "";
            txtilgilikisi.Text = "";
            txtsipno.Text = "";
            lkuefrimaunvan.EditValue = string.Empty;
            raddovizfatura.Checked = true;
            rabucretalıcı.Checked = true;
            sırasayısı = 0;
            tablo.Rows.Clear();

            stokbilgileritemizle();
        }

        void carilerdoldur()
        {
            try
            {
                OleDbCommand komut = new OleDbCommand("select * from [cariler$]", bgl.excelcarilerzconnection());
                OleDbDataReader reader = komut.ExecuteReader();
                while (reader.Read())
                {
                    //düzeltilecek alan
                    firmaunvan.Add(reader[0] + "-) " + reader[1].ToString());
                }
                reader.Close();
                bgl.excelcarilerzconnection().Close();
                lkuefrimaunvan.Properties.DataSource = firmaunvan;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Exit();
            }
        }

        void tetikle()
        {
            Thread stokbilgi = new Thread(new ThreadStart(stokbilgileridoldur));
            stokbilgi.Start();
            stokbilgi.Join();

            Thread digercombo = new Thread(new ThreadStart(digercomboliste));
            digercombo.Start();
            digercombo.Join();

            carilerdoldur();
            herseyitemizle();
        }

        Font cozunurlukayarla()
        {
            //çözünürlük işlemleri
            float genOra = kullanilanPcGenislik / bizimPcGenislik;
            float yukOra = kullanilanPcYukseklik / bizimPcYukseklik;
            Font font = new Font(layoutControlItem1.AppearanceItemCaption.Font.Name, 14);
            float forGen = Size.Width;
            float forYuk = Size.Height;
            foreach (Control nesne in layoutControl1.Controls)
            {
                string nesFontAdi = nesne.Font.SystemFontName;
                float nesFont = nesne.Font.Size;
                float nesX = nesne.Location.X;
                float nesY = nesne.Location.Y;
                float nesGen = nesne.Size.Width;
                float nesYuk = nesne.Size.Height;
                nesne.Location = new Point((int)(nesX * genOra), (int)(nesY * yukOra));
                nesne.Size = new Size((int)(nesGen * genOra), (int)(nesYuk * yukOra));
                int fontBuyuk = (int)(nesFont * yukOra);
                font = new Font(nesFontAdi, fontBuyuk);
            }
            return font;
        }
        #endregion

        #region combobox doldurmak ve diğer işlemler
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                herseyitemizle();
                sırasayısı = 0;
                satıreklemi = true;
                guncelenebilirmi = false;
                mailgonderilsinmi = true;
                tablo.Font = new Font("Times New Roman", 14, FontStyle.Regular);

                //bu kısımda proje farklı çözünürlükdeki bilgisayarlarda düzgün gözükmesi için bütün araçların fontunu çözünürlüğe oranlayan bir algoritma yazdım.
                float genOra = kullanilanPcGenislik / bizimPcGenislik;
                float yukOra = kullanilanPcYukseklik / bizimPcYukseklik;
                SizeF size = new SizeF(genOra, yukOra);
                Scale(size);
                float forGen = Size.Width;
                float forYuk = Size.Height;
                foreach (Control nesne in layoutControl1.Controls)
                {
                    string nesFontAdi = nesne.Font.SystemFontName;
                    float nesFont = nesne.Font.Size;
                    float nesX = nesne.Location.X;
                    float nesY = nesne.Location.Y;
                    float nesGen = nesne.Size.Width;
                    float nesYuk = nesne.Size.Height;
                    nesne.Location = new Point((int)(nesX * genOra), (int)(nesY * yukOra));
                    nesne.Size = new Size((int)(nesGen * genOra), (int)(nesYuk * yukOra));
                    int fontBuyuk = (int)(nesFont * yukOra);
                    if (fontBuyuk < 8) fontBuyuk = 8;//Yazı en küçük 8 punto olsun
                    nesne.Font = new Font(nesFontAdi, fontBuyuk);
                    if (nesne is ListView) Size = new Size((int)(nesGen * genOra + 30), (int)(forYuk * yukOra));
                }
                foreach (Control nesne in groupControl1.Controls)
                {
                    string nesFontAdi = nesne.Font.SystemFontName;
                    float nesFont = nesne.Font.Size;
                    float nesX = nesne.Location.X;
                    float nesY = nesne.Location.Y;
                    float nesGen = nesne.Size.Width;
                    float nesYuk = nesne.Size.Height;
                    nesne.Location = new Point((int)(nesX * genOra), (int)(nesY * yukOra));
                    nesne.Size = new Size((int)(nesGen * genOra), (int)(nesYuk * yukOra));
                    int fontBuyuk = (int)(nesFont * yukOra);
                    if (fontBuyuk < 8) fontBuyuk = 8;//Yazı en küçük 8 punto olsun
                    nesne.Font = new Font(nesFontAdi, fontBuyuk);
                    if (nesne is ListView) Size = new Size((int)(nesGen * genOra + 30), (int)(forYuk * yukOra));
                }
                foreach (Control nesne in groupControl2.Controls)
                {
                    string nesFontAdi = nesne.Font.SystemFontName;
                    float nesFont = nesne.Font.Size;
                    float nesX = nesne.Location.X;
                    float nesY = nesne.Location.Y;
                    float nesGen = nesne.Size.Width;
                    float nesYuk = nesne.Size.Height;
                    nesne.Location = new Point((int)(nesX * genOra), (int)(nesY * yukOra));
                    nesne.Size = new Size((int)(nesGen * genOra), (int)(nesYuk * yukOra));
                    int fontBuyuk = (int)(nesFont * yukOra);
                    if (fontBuyuk < 8) fontBuyuk = 8;//Yazı en küçük 8 punto olsun
                    nesne.Font = new Font(nesFontAdi, fontBuyuk);
                    if (nesne is ListView) Size = new Size((int)(nesGen * genOra + 30), (int)(forYuk * yukOra));
                }

                tablo.Columns.Add("sırasayisi", "");
                tablo.Columns.Add("stokbilgi", "stok bilgisi");
                tablo.Columns.Add("Miktar", "Miktar");
                tablo.Columns.Add("birim fiyat", "Birim Fiyat");
                tablo.Columns.Add("tutar", "Tutar");
                tablo.Columns[1].Width = tablo.Size.Width * 50 / 100;
                tablo.Columns[0].Width = tablo.Size.Width * 3 / 100;

                tetikle();

            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Giriş yapılamadı lütfen daha sonra tekrar deneyiniz. eğer aynı hattayı tekrar görürseniz iletişime geçiniz " + ex, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
        #endregion

        #region silme ve güncelleme işlemleri
        private void tablo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            secilensatır = e.RowIndex;

            try
            {
                if (tablo.Rows[e.RowIndex].Cells[3].Value != null)
                {
                    satıreklemi = false;
                    guncelenebilirmi = true;
                    txtbirimfiyat.Text = tablo.Rows[e.RowIndex].Cells[3].Value.ToString();
                }
                if (tablo.Rows[e.RowIndex].Cells[1].Value != null)
                    lkupstokbilgi.Text = tablo.Rows[e.RowIndex].Cells[1].Value.ToString();
                if (tablo.Rows[e.RowIndex].Cells[2].Value != null)
                    txtmiktar.Text = tablo.Rows[e.RowIndex].Cells[2].Value.ToString();
                int i = lkupstokbilgi.ItemIndex;
                lblstokmiktari.Text = "Stok:" + stokmiktarı[i].ToString();
            }
            catch (Exception)
            {

            }

        }

        //günceleme işlemi
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (guncelenebilirmi == true)
                {

                    if (tablo.Rows[secilensatır].Cells[4].Value != null)
                    {
                        toplam -= decimal.Parse(tablo.Rows[secilensatır].Cells[4].Value.ToString());
                    }
                    satıreklemi = true;
                    guncelenebilirmi = false;
                    if (sırasayısı == 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Herhangi bir satır eklenmeden günceleme işlemi yapılamaz.", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        if (tablo.Rows[secilensatır].Cells[3].Value != null)
                            tablo.Rows[secilensatır].Cells[3].Value = txtbirimfiyat.Text;
                        if (tablo.Rows[secilensatır].Cells[1].Value != null)
                            tablo.Rows[secilensatır].Cells[1].Value = lkupstokbilgi.Text;
                        if (tablo.Rows[secilensatır].Cells[2].Value != null)
                            tablo.Rows[secilensatır].Cells[2].Value = txtmiktar.Text;

                        miktar = int.Parse(txtmiktar.Text);
                        birimfiyat = decimal.Parse(txtbirimfiyat.Text);
                        tutar = miktar * birimfiyat;

                        if (tablo.Rows[secilensatır].Cells[4].Value != null)
                            tablo.Rows[secilensatır].Cells[4].Value = tutar;

                        if (sırasayısı == 1)
                            toplam = decimal.Parse(tablo.Rows[0].Cells[4].Value.ToString());
                        else
                            toplam += decimal.Parse(tablo.Rows[sırasayısı - 1].Cells[4].Value.ToString());

                        txttoplam.Text = toplam.ToString();
                        stokbilgileritemizle();
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Herhangi bir satır seçilmeden güncelleme yapılamaz.", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Güncelleme işlemi yapılırken beklenmedik bir sorunla karşılaşıldı. \n lütfen daha sonra tekrar deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //silme işlemi
        private void btnsil_Click(object sender, EventArgs e)
        {
            try
            {
                if (guncelenebilirmi == true)
                {
                    if (tablo.Rows[secilensatır].Cells[4].Value != null)
                    {
                        toplam -= decimal.Parse(tablo.Rows[secilensatır].Cells[4].Value.ToString());
                    }
                    satıreklemi = true;
                    guncelenebilirmi = false;
                    if (sırasayısı < 1)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Herhangi bir satır eklenmeden silme işlemi yapılamaz.", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        DataGridViewRow data = tablo.Rows[secilensatır];
                        tablo.Rows.Remove(data);
                        stokbilgileritemizle();
                        sırasayısı = sırasayısı - 1;

                        for (int i = 1; i <= sırasayısı; i++)
                        {
                            tablo.Rows[i - 1].Cells[0].Value = i;
                        }
                    }
                    txttoplam.Text = toplam.ToString();
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Herhangi bir satır seçilmeden silme yapılamaz.", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Silme işlemi yapılırken beklenmedik bir sorunla karşılaşıldı. \n lütfen daha sonra tekrar deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        #endregion

        #region mail atma işlemi

        private void radtlfatura_CheckedChanged(object sender, EventArgs e)
        {
            if (radtlfatura.Checked == true)
                faturaturu = radtlfatura.Text;
        }

        private void raddovizfatura_CheckedChanged(object sender, EventArgs e)
        {
            if (raddovizfatura.Checked == true)
                faturaturu = raddovizfatura.Text;
        }

        private void rabucretalıcı_CheckedChanged(object sender, EventArgs e)
        {
            if (rabucretalıcı.Checked == true)
                kargoodeme = rabucretalıcı.Text;

        }

        private void rabpesinodeme_CheckedChanged(object sender, EventArgs e)
        {
            if (rabpesinodeme.Checked == true)
                kargoodeme = rabpesinodeme.Text;
        }

        private void btngonder_Click(object sender, EventArgs e)
        {
            try
            {
                bosgecilenalanlar = "";
                mailgonderilsinmi = true;
                if (lkuefrimaunvan.Text == "")
                {
                    bosgecilenalanlar += "Frima Ünvan\n";
                    mailgonderilsinmi = false;
                }

                if (cmbplasiyeradi.Text == "")
                {
                    bosgecilenalanlar += "Plasiyer Adı\n";
                    mailgonderilsinmi = false;
                }

                if (txtilgilikisi.Text == "")
                {
                    bosgecilenalanlar += "İlgili Kişi\n";
                    mailgonderilsinmi = false;
                }

                if (cmbteslimatsekl.Text == "")
                {
                    mailgonderilsinmi = false;
                    bosgecilenalanlar += "Teslimat Şekli\n";
                }

                if (txtteslimatadrs.Text == "")
                {
                    mailgonderilsinmi = false;
                    bosgecilenalanlar += "Teslimat Adresi\n";
                }

                if (sırasayısı == 0)
                {
                    satireklenmis = false;
                    DevExpress.XtraEditors.XtraMessageBox.Show("Hiç bir satır eklenmeden fatura gönderme işlemi yapılamaz.", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    satireklenmis = true;

                if (mailgonderilsinmi == false && satireklenmis == true)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Lütfen şu alanları konrol ettikten sonra tekrar deneyiniz.\n" + bosgecilenalanlar, "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (mailgonderilsinmi == true && satireklenmis == true)
                {
                    //html kodları
                    htmlkodu = "<table border = \"1\" width = \"1000\"> <tr> <td height =\"100\">Stok bilgisi</td> <td height =\"25\">miktar</td> <td height =\"25\">Birim Fiyat </td> <td height =\"25\">Tutar</td> </tr> \n";
                    for (int i = 0; i <= tablo.Rows.Count - 1; i++)
                    {
                        if (tablo.Rows[i].Cells[1].Value != null && tablo.Rows[i].Cells[2].Value != null && tablo.Rows[i].Cells[3].Value != null && tablo.Rows[i].Cells[4].Value != null)
                        {
                            htmlkodu += "<tr> <td height =\"25\">" + tablo.Rows[i].Cells[1].Value.ToString() + " <td height =\"25\" >" + tablo.Rows[i].Cells[2].Value.ToString() + "</td> <td height =\"25\" >" + tablo.Rows[i].Cells[3].Value.ToString() + " " + cmbdovizturu.Text + "</td> <td height =\"25\" >" + tablo.Rows[i].Cells[4].Value.ToString() + " " + cmbdovizturu.Text + "</td> </tr> \n";
                        }

                    }
                    htmlkodu += "</table>";
                    htmlkodu += "<font size = \"6\" > <b> Toplam Tutar :" + toplam + cmbdovizturu.Text + "</b> </font> \n";
                    mail = "<p>Firma Ünvan : " + lkuefrimaunvan.Text + "</p>";
                    mail += "<p>Plasiyer Adı : " + cmbplasiyeradi.Text + "</p>";
                    mail += "<p>Döviz : " + cmbdovizturu.Text + "</p>";
                    mail += "<p>Vade : " + Cmbvade.Text + "</p>";
                    mail += "<p>İlgili Kişi : " + txtilgilikisi.Text + "</p>";
                    mail += "<p>İlgili Kişi Tel : " + txtilgilikisitel.Text + "</p>";
                    mail += "<p>Sipariş Numarası : " + txtsipno.Text + "</p>";
                    mail += "<p>Teslimat Şekli : " + cmbteslimatsekl.Text + "</p>";
                    mail += "<p>Fatura Türü : " + faturaturu + "</p>";
                    if (kargoodeme != null)
                    {
                        mail += "<p>Kargo ücreti : " + kargoodeme + "</p>";
                    }
                    mail += "<p>Teslimat Adresi : " + txtteslimatadrs.Text + "</p>";
                    mail += "<p>Notlar :" + txtnotlar.Text + "<p/>";

                    //mail atma
                    MailMessage mesaj = new MailMessage();
                    SmtpClient mailslemci = new SmtpClient();
                    mailslemci.Credentials = new NetworkCredential("giden mail adresi", "giden mail adresi şifre");
                    mailslemci.Port = 587;
                    mailslemci.Host = "göndren e postanın hizmeti"; // outlook : smtp.live.com  Gmail : smtp.gmail.com  yandex : smtp.yandex.com
                    mailslemci.EnableSsl = true;
                    mesaj.To.Add("kime gideceği");
                    mesaj.From = new MailAddress("mesajın hangi mail adresinden geldiği (giden mail adresi ile aynı olmalı)");
                    mesaj.IsBodyHtml = true;
                    mesaj.Subject = cmbplasiyeradi.Text + " - " + lkuefrimaunvan.Text;
                    mesaj.Body = mail + "\n" + htmlkodu;
                    mailslemci.Send(mesaj);
                    DevExpress.XtraEditors.XtraMessageBox.Show("Fatura başarıyla gönderildi.", "BİLGİ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    herseyitemizle();
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Fatura gönderilemedi. Lütfen internet bağlantınız kontrol edip tekrar deneyiniz.\n\n" + ex, "HATTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region diğer işlemler.

        private void cmbteslimatsekl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbteslimatsekl.SelectedIndex == 0 || cmbteslimatsekl.SelectedIndex == 1 || cmbteslimatsekl.SelectedIndex == 2)
            {
                rabpesinodeme.Enabled = false;
                rabucretalıcı.Enabled = false;
                rabpesinodeme.Checked = false;
                rabucretalıcı.Checked = false;
                kargoodeme = null;
            }
            else
            {
                rabpesinodeme.Enabled = true;
                rabucretalıcı.Enabled = true;
                rabucretalıcı.Checked = true;
                rabpesinodeme.Checked = false;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            stokbilgileritemizle();
        }

        private void btntemizle_Click(object sender, EventArgs e)
        {
            herseyitemizle();
        }

        private void textEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtbirimfiyat_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ',';
        }

        private void txtmiktar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (kontrol == false)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Böyle bir stok kodu sistemde bulunamadı.", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (satıreklemi == true)
                    {
                        if (txtbirimfiyat.Text == "")
                        {
                            miktar = int.Parse(txtmiktar.Text);
                        }
                        else if (txtbirimfiyat.Text != "")
                        {
                            miktar = int.Parse(txtmiktar.Text);
                            birimfiyat = decimal.Parse(txtbirimfiyat.Text);
                            tutar = miktar * birimfiyat;
                            satırekle();
                            stokbilgileritemizle();
                        }
                    }

                }
            }
        }

        private void txtbirimfiyat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (kontrol == false)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Böyle bir stok kodu sistemde bulunamadı.", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (satıreklemi == true)
                    {
                        if (txtmiktar.Text == "")
                        {
                            birimfiyat = decimal.Parse(txtbirimfiyat.Text);
                        }
                        else if (txtmiktar.Text != "")
                        {
                            miktar = int.Parse(txtmiktar.Text);
                            birimfiyat = decimal.Parse(txtbirimfiyat.Text);
                            tutar = miktar * birimfiyat;
                            satırekle();
                            stokbilgileritemizle();
                        }
                    }
                }
            }

        }

        private void textEdit1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ';
        }


        private void lkupstokbilgi_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (lkupstokbilgi.Text != null)
                {
                    lblstokmiktari.Text = "Stok :" + stokmiktarı[lkupstokbilgi.ItemIndex];
                    kontrol = true;
                }
            }
            catch (Exception)
            {
                kontrol = false;
            }

        }

        private void lkupstokbilgi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (kontrol == false)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Böyle bir stok kodu sistemde bulunamadı.", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void font_degistir(object sender, EventArgs e)
        {
            ((DevExpress.XtraLayout.LayoutItem)sender).AppearanceItemCaption.Font = cozunurlukayarla();
        }

        private void btninfo_Click(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.XtraMessageBox.Show("bu proje Ahmet Beyazıt tarafından özel olarak geliştirilmiştir.", "BİLGİ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}



