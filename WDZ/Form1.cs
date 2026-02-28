using System.Runtime.InteropServices;
using Timer = System.Windows.Forms.Timer;
using System.IO;
using System.Diagnostics;
using System.Text.Json;

namespace WDZ
{
    public partial class Form1 : Form
    {
        private const int HOTKEY_ID = 1;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_ALT = 0x0001;
        private const uint VK_Q = 0x51;
        private Timer hoverTimer = new Timer();
        private PictureBox currentPB;
        private int targetSize;
        private int normalSize = 32;
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect,
        int nTopRect,
        int nRightRect,
        int nBottomRect,
        int nWidthEllipse,
        int nHeightEllipse
    );

        private void AbrirExe(object sender)
        {
            string caminho;
            if (sender is Panel pnl)
                caminho = (string)pnl.Tag;
            else if (sender is PictureBox pb && pb.Parent is Panel pnlParent)
                caminho = (string)pnlParent.Tag;
            else
                return;

            if (File.Exists(caminho))
                System.Diagnostics.Process.Start(caminho);
            else
                MessageBox.Show("Arquivo não encontrado: " + caminho);

        }

        public Form1()
        {
            InitializeComponent();

        }
        private Timer slideTimer = new Timer { Interval = 10 };
        private bool showing = false;
        private int slideTarget = 20;
        private int slideStep = 5;

        private void ToggleWindow()
        {
            showing = !showing;

            if (showing)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;

                // Força prioridade real
                this.TopMost = false;
                this.TopMost = true;

                this.BringToFront();
                this.Activate();

                ShowWindow(this.Handle, 5);
                SetForegroundWindow(this.Handle);

                slideTimer.Start();
            }
            else
            {
                slideTimer.Start();
            }
        }

        private void AumentarIcone(PictureBox pb, Icon icone, bool aumentar)
        {
            currentPB = pb;
            targetSize = aumentar ? 42 : normalSize;
            hoverTimer.Start();
        }
        private void SalvarConfig()
        {
            try
            {
                var caminhos = new List<string>();

                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel pnl && pnl.Tag is string caminho)
                    {
                        caminhos.Add(caminho);
                    }
                }

                string json = System.Text.Json.JsonSerializer.Serialize(caminhos);
                string path = Path.Combine(Application.StartupPath, "apps.json");
                File.WriteAllText(path, json);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar configuração: " + ex.Message);
            }
        }

        private void CarregarConfig()
        {
            try
            {
                if (File.Exists("apps.json"))
                {
                    string json = File.ReadAllText("apps.json");
                    var caminhos = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json);

                    foreach (string caminho in caminhos)
                    {
                        Panel pnl = CriarBotaoExe(caminho);
                        this.Controls.Add(pnl);
                    }

                    ReorganizarBotoes();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar configuração: " + ex.Message);
            }
        }


        private Panel CriarBotaoExe(string caminhoExe)
        {
            Panel pnl = new Panel();
            pnl.Width = 50;
            pnl.Height = 50;
            pnl.BackColor = Color.Transparent;

            Icon icone = Icon.ExtractAssociatedIcon(caminhoExe);
            PictureBox pb = new PictureBox();
            pb.Image = new Icon(icone, new Size(32, 32)).ToBitmap();
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.Width = 32;
            pb.Height = 32;

            void CentralizarIcone()
            {
                pb.Left = (pnl.Width - pb.Width) / 2;
                pb.Top = (pnl.Height - pb.Height) / 2;
            }

            CentralizarIcone();
            pnl.Resize += (s, e) => CentralizarIcone();

            pnl.Controls.Add(pb);

            pnl.Tag = caminhoExe;

            Timer localTimer = new Timer { Interval = 40 };
            int normalSize = 32;
            int hoverSize = 42;
            int targetSize = normalSize;

            localTimer.Tick += (s, e) =>
            {
                int diff = targetSize - pb.Width;
                if (diff == 0)
                {
                    localTimer.Stop();
                    return;
                }

                int passo = Math.Sign(diff) * 2;
                int novoTamanho = pb.Width + passo;
                if ((passo > 0 && novoTamanho > targetSize) || (passo < 0 && novoTamanho < targetSize))
                    novoTamanho = targetSize;

                pb.Width = novoTamanho;
                pb.Height = novoTamanho;
                CentralizarIcone();
            };

            pb.MouseEnter += (s, e) =>
            {
                targetSize = hoverSize;
                localTimer.Start();
            };

            pb.MouseLeave += (s, e) =>
            {
                targetSize = normalSize;
                localTimer.Start();
            };

            void AbrirExe()
            {
                if (File.Exists(caminhoExe))
                    Process.Start(caminhoExe);
                else
                    MessageBox.Show("Arquivo não encontrado!");
            }

            pnl.Click += (s, e) => AbrirExe();
            pb.Click += (s, e) => AbrirExe();

            return pnl;
        }




        private void ReorganizarBotoes()
        {
            int spacing = 5;
            int posX = spacing + 50;

            foreach (Control ctrl in this.Controls.OfType<Panel>())
            {
                ctrl.Left = posX;
                ctrl.Top = 0;
                posX += ctrl.Width + spacing;
            }
        }



        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Executáveis (*.exe)|*.exe";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string caminhoExe = ofd.FileName;
                    Panel pnl = CriarBotaoExe(caminhoExe);
                    pnl.Left = 100;
                    pnl.Top = 5;
                    this.Controls.Add(pnl);
                    pnl.BringToFront();
                    ReorganizarBotoes();
                    SalvarConfig();


                }
            }
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                dragCursorPoint = Cursor.Position;
                dragFormPoint = this.Location;
            }
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(diff));
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int largura = (int)(Screen.PrimaryScreen.Bounds.Width * 0.8);
            int altura = 50;
            int margemTopo = 20;

            this.Width = largura;
            this.Height = altura;
            this.Left = (Screen.PrimaryScreen.Bounds.Width - largura) / 2;
            this.Top = -this.Height;
            slideTimer.Tick += SlideTimer_Tick;
            bool hotkeyRegistered = RegisterHotKey(this.Handle, HOTKEY_ID, MOD_SHIFT | MOD_ALT, VK_Q);
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 10, 10));
            this.Resize += Form1_Resize;

            Button btnAdd = new Button();
            btnAdd.Text = "+";
            btnAdd.Width = 50;
            btnAdd.Height = 50;
            btnAdd.Left = 5;
            btnAdd.Top = 0;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.BackColor = Color.Transparent;
            btnAdd.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnAdd.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnAdd.Click += BtnAdd_Click;

            this.Controls.Add(btnAdd);

            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;

            CarregarConfig();
            bool registered = RegisterHotKey(this.Handle, HOTKEY_ID, MOD_SHIFT | MOD_ALT, VK_Q);

        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                Hubris.Visible = true;
                Hubris.BalloonTipText = "WDZ está rodando em segundo plano!";
                Hubris.ShowBalloonTip(1000);
            }
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                cp.ExStyle &= ~0x40000;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
            {
                ToggleWindow();
            }
            base.WndProc(ref m);
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Hubris.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    Hubris.Visible = false;
                }
            };


        }
        private void SlideTimer_Tick(object sender, EventArgs e)
        {
            int passo = 5;

            if (showing)
            {
                if (this.Top < slideTarget)
                    this.Top += passo;
                else
                {
                    this.Top = slideTarget;
                    slideTimer.Stop();
                }
            }
            else
            {
                if (this.Top > -this.Height)
                    this.Top -= passo;
                else
                {
                    this.Top = -this.Height;
                    slideTimer.Stop();
                }
            }
        }

        private void MinimizarParaTray()
        {
            this.Hide();
            Hubris.Visible = true;
            Hubris.BalloonTipText = "WDZ está rodando em segundo plano!";
            Hubris.ShowBalloonTip(1000);
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Show/Hide", null, (s, e) => this.Visible = !this.Visible);
            menu.Items.Add("Exit", null, (s, e) => Application.Exit());

            Hubris.ContextMenuStrip = menu;

        }

    }
}
