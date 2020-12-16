using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using wpfScrapingRegister.collection;
using wpfScrapingRegister.dao;
using wpfScrapingRegister.data;
using WpfScrapingRegister.collection;
using WpfScrapingRegister.common;
using WpfScrapingRegister.dao;

namespace wpfScrapingRegister
{
    public class IntCollection : ObservableCollection<int> { }
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int IS_SELECTION_DEFAULT = 0;
        private const int IS_SELECTION_NOT_TARGET = -1;
        private const int IS_SELECTION_TARGET = 1;
        private const int IS_SELECTION_TARGETCOPY = 2;
        private const int IS_SELECTION_WAITING_HD = 3;
        private const int IS_SELECTION_DONE = 9;

        private const int DATAGRID_JAV = 1;
        private const int DATAGRID_BJ = 2;
        private int dispctrlDataGrid = 0;

        private JavCollection colJav;
        private JavData dispinfoSelectJavData = null;
        private JavDao javDao = null;

        private string[] dispinfoThumbnailsArray = null;
        private int dispinfoCurrentThumbnail = 0;

        private JavCollection colJavCheck;
        private JavData dispinfoSelectJavCheck = null;
        private Jav2Data dispinfoSelectJav2 = null;

        private BjCollection colBj;
        private BjData dispinfoSelectBjData = null;
        private BjDao bjDao = null;

        private Jav2Collection colJav2;
        private Jav2Dao jav2Dao = null;

        private MakerCollection colMaker;
        private MakerDao makerDao = null;

        private FileCollection colFile;

        private string ImagePath = "";

        private MySqlDbConnection dockerMySqlConn = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                dockerMySqlConn = new MySqlDbConnection(0);

                javDao = new JavDao();
                jav2Dao = new Jav2Dao();
                bjDao = new BjDao();
                makerDao = new MakerDao();

                colJav = new JavCollection(javDao.GetList());
                dgridJav.ItemsSource = colJav.collecion;

                colJavCheck = new JavCollection(javDao.GetList());
                dgridJavCheck.ItemsSource = colJavCheck.collecion;

                colBj = new BjCollection(bjDao.GetList());
                dgridBj.ItemsSource = colBj.collecion;

                colJav2 = new Jav2Collection(jav2Dao.GetList());
                dgridJav2.ItemsSource = colJav2.collecion;

                colMaker = new MakerCollection(makerDao.GetList());

                colFile = new FileCollection();
                dgridFiles.ItemsSource = colFile.collecion;

                dispctrlDataGrid = DATAGRID_JAV;
                SwitchDataGrid(dispctrlDataGrid);

                this.Title = "ScrapingRegister " + colJav.GetIsSelectionZero();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void OnLayoutSizeChanged(object sender, SizeChangedEventArgs e)
        {
            txtStatusBar.IsReadOnly = true;
            txtStatusBar.Width = stsbaritemDispDetail.ActualWidth;
        }


        private void SwitchDataGrid(int myVisibilityGrid)
        {
            if (myVisibilityGrid == 1)
            {
                dgridJav.Visibility = Visibility.Visible;
                dgridBj.Visibility = Visibility.Collapsed;
                imageThumbnail.Visibility = Visibility.Visible;
                imageThumbnail1.Visibility = Visibility.Collapsed;
                imageThumbnail2.Visibility = Visibility.Collapsed;
                imageThumbnail3.Visibility = Visibility.Collapsed;
                imageThumbnail4.Visibility = Visibility.Collapsed;

                ImagePath = @"C:\mydata\jav-save";
            }
            else
            {
                dgridJav.Visibility = Visibility.Collapsed;
                dgridBj.Visibility = Visibility.Visible;
                imageThumbnail.Visibility = Visibility.Collapsed;
                ImagePath = @"C:\mydata\bj-jpeg";
            }
        }

        private void dgridJavCheck_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgridJavCheck.SelectedItem == null)
                return;

            dispinfoSelectJavCheck = (JavData)dgridJavCheck.SelectedItem;

        }

        private void dgridJav_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgridJav.SelectedItem == null)
                return;

            this.Title = "ScrapingRegister " + colJav.GetIsSelectionZero();
            dispinfoSelectJavData = (JavData)dgridJav.SelectedItem;

            txtStatusBar.Text = dispinfoSelectJavData.Title;
            txtRegisterProductNumber.Text = dispinfoSelectJavData.ProductNumber;
            txtRegisterActress.Text = dispinfoSelectJavData.Actress;
            txtRegisterMaker.Text = dispinfoSelectJavData.Maker;
            txtRegisterLabel.Text = dispinfoSelectJavData.Label;
            txtRegisterSellDate.Text = dispinfoSelectJavData.SellDate.ToString("yyyy/MM/dd");
            txtRegisterPackage.Text = dispinfoSelectJavData.Package;
            txtRegisterThumbnail.Text = dispinfoSelectJavData.Thumbnail;
            txtRegisterDownloadFiles.Text = dispinfoSelectJavData.DownloadFiles;
            txtRegisterFilesInfo.Text = dispinfoSelectJavData.FilesInfo;

            cmbLargeRating.SelectedValue = dispinfoSelectJavData.Rating;

            txtRegisterMatch.Text = colMaker.GetMatchStr(txtRegisterProductNumber.Text.Split('-')[0]);

            if (!string.IsNullOrEmpty(dispinfoSelectJavData.ProductNumber))
            {
                txtJav2Search.Text = dispinfoSelectJavData.ProductNumber;

                btnJav2Search_Click(null, null);

                colJavCheck.SetSearchText(txtJav2Search.Text);
                colJavCheck.Execute();
            }

            if (!string.IsNullOrEmpty(dispinfoSelectJavData.ProductNumber))
            {
                string pNumber = dispinfoSelectJavData.ProductNumber.Replace("-", "");

                string fileSearch = pNumber + " " + dispinfoSelectJavData.ProductNumber;
                colFile.SetSearchText(fileSearch);
                colFile.Execute();
            }

            string filname = "";
            dispinfoThumbnailsArray = dispinfoSelectJavData.Thumbnail.Split(' ');

            if (dispinfoThumbnailsArray.Length > 0)
            {
                TxtbThumbnailCount.Text = Convert.ToString(dispinfoThumbnailsArray.Length);
                if (dispinfoSelectJavData.Thumbnail.IndexOf("http", StringComparison.Ordinal) == 0)
                    filname = System.IO.Path.GetFileName(dispinfoThumbnailsArray[0]);
                else
                    filname = dispinfoThumbnailsArray[0];

                dispinfoCurrentThumbnail = 0;
            }

            string pathname = System.IO.Path.Combine(ImagePath, filname);

            if (System.IO.File.Exists(pathname))
            {
                imageThumbnail.Source = this.GetImageStream(pathname);

                if (imageThumbnail.Source != null)
                {
                    BitmapImage bitmapImage = (BitmapImage)imageThumbnail.Source;
                    imageThumbnail.Width = lgridMain.ColumnDefinitions[1].ActualWidth;
                    imageThumbnail.Height = (imageThumbnail.Width / bitmapImage.Width) * bitmapImage.Height;
                    imageThumbnail.Stretch = Stretch.Uniform;
                }
            }

            if (dispinfoSelectJavData.Package.IndexOf("http", StringComparison.Ordinal) == 0)
                filname = System.IO.Path.GetFileName(dispinfoSelectJavData.Package);
            else
                filname = dispinfoSelectJavData.Package;

            pathname = System.IO.Path.Combine(ImagePath, filname);

            if (System.IO.File.Exists(pathname))
            {
                imagePackage.Source = this.GetImageStream(pathname);
                imagePackage.Stretch = Stretch.Uniform;
            }

            txtStatusBarId.Text = Convert.ToString(dispinfoSelectJavData.Id);

            txtDownloadLinks.Text = dispinfoSelectJavData.DownloadLinks;

            if (txtRegisterActress.Text.Length > 0)
            {
                AvContentsDao contentsDao = new AvContentsDao();
                char splitChar = Actress.GetSplitChar(txtRegisterActress.Text);
                string[] arrActress = txtRegisterActress.Text.Split(splitChar);
                try
                {
                    txtStatusBar.Text = Actress.GetEvaluation(String.Join(",", arrActress), contentsDao, dockerMySqlConn);
                }
                catch (MySqlException emysql)
                {
                    txtStatusBar.Text = emysql.Message;
                }
                catch (Exception ex)
                {
                    txtStatusBar.Text = "exception " + ex.Message;
                }
            }
        }

        public BitmapImage GetImageStream(string myImagePathname)
        {
            if (!System.IO.File.Exists(myImagePathname))
                return null;

            BitmapImage bitmap = new BitmapImage();
            FileStream stream = null;
            try
            {
                stream = System.IO.File.OpenRead(myImagePathname);
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }
            // このピクセル形式に関する情報は見つかりませんでした。
            catch (NotSupportedException ex)
            {
                Debug.Write(ex);
                return null;
            }
            catch(IOException ex)
            {
                Debug.Write(ex);
                return null;
            }
            stream.Close();
            stream.Dispose();
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            return bitmap;
        }


        private (int isSelection, bool isCopy) GetMenuItemIsSelection(MenuItem menuitem)
        {
            string menuitemHeader = menuitem.Header.ToString();

            int isSelection = 0;
            bool isCopy = false;
            if (menuitemHeader.Equals("対象外"))
                isSelection = IS_SELECTION_NOT_TARGET;
            else if (menuitemHeader.Equals("対象")
                     || menuitemHeader.Equals("対象COPY"))
            {
                isSelection = IS_SELECTION_TARGET;

                if (menuitemHeader.Equals("対象COPY"))
                    isCopy = true;
            }
            else if (menuitemHeader.Equals("HD待ち"))
            {
                isSelection = IS_SELECTION_WAITING_HD;
            }
            else if (menuitemHeader.Equals("初期値0"))
                isSelection = IS_SELECTION_DEFAULT;

            return (isSelection, isCopy);
        }
        private void OnClickMenuitemSelectTarget(object sender, RoutedEventArgs e)
        {
            MenuItem menuitem = sender as MenuItem;

            (int isSelection, bool isCopy)resultTuple = GetMenuItemIsSelection(menuitem);

            if (dispctrlDataGrid == DATAGRID_JAV)
            {
                UpdateJavDownloadFiles(dispinfoSelectJavData.DownloadLinks);

                javDao.UpdateIsSelection(resultTuple.isSelection, dispinfoSelectJavData.Id);
                dispinfoSelectJavData.IsSelection = resultTuple.isSelection;
                colJavCheck.SetDataIsSelection(dispinfoSelectJavData.Id, resultTuple.isSelection);
            }
            else
            {
                bjDao.UpdateIsSelection(resultTuple.isSelection, dispinfoSelectBjData.Id);
                dispinfoSelectBjData.IsSelection = resultTuple.isSelection;
            }

            if (resultTuple.isCopy)
                CopyClipboard(txtDownloadLinks.Text);
        }

        private void OnClickMenuitemSelectTargetDataGridCheck(object sender, RoutedEventArgs e)
        {
            MenuItem menuitem = sender as MenuItem;

            UpdateJavDownloadFiles(dispinfoSelectJavCheck.DownloadLinks);

            (int isSelection, bool isCopy) resultTuple = GetMenuItemIsSelection(menuitem);

            javDao.UpdateIsSelection(resultTuple.isSelection, dispinfoSelectJavCheck.Id);

            dispinfoSelectJavCheck.IsSelection = resultTuple.isSelection;
            colJav.SetDataIsSelection(dispinfoSelectJavCheck.Id, resultTuple.isSelection);
        }

        private void dgridBj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dispinfoSelectBjData = (BjData)dgridBj.SelectedItem;

            if (dispinfoSelectBjData == null)
                return;

            string[] arrThumbnail = dispinfoSelectBjData.Thumbnails.Split(' ');
            List<string> arrPathname = new List<string>();
            foreach (string thumbnail in arrThumbnail)
            {
                string filname = "";
                if (thumbnail.IndexOf("http") == 0)
                    filname = System.IO.Path.GetFileName(thumbnail);
                else
                    filname = thumbnail;

                string pathname = System.IO.Path.Combine(ImagePath, System.IO.Path.GetFileName(filname));
                if (System.IO.File.Exists(pathname))
                    arrPathname.Add(pathname);
            }

            Image[] arrImage = new Image[arrPathname.Count];
            imageThumbnail1.Visibility = Visibility.Collapsed;
            imageThumbnail2.Visibility = Visibility.Collapsed;
            imageThumbnail3.Visibility = Visibility.Collapsed;
            imageThumbnail4.Visibility = Visibility.Collapsed;
            if (arrPathname.Count >= 1)
            {
                arrImage[0] = imageThumbnail1;
                imageThumbnail1.Visibility = Visibility.Visible;
                if (arrPathname.Count == 1)
                    imageThumbnail1.SetValue(Grid.RowSpanProperty, 4);
            }
            if (arrPathname.Count >= 2)
            {
                arrImage[1] = imageThumbnail2;
                imageThumbnail2.Visibility = Visibility.Visible;
                if (arrPathname.Count == 2)
                {
                    imageThumbnail1.SetValue(Grid.RowSpanProperty, 2);
                    imageThumbnail2.SetValue(Grid.RowSpanProperty, 2);
                }
            }
            if (arrPathname.Count >= 3)
            {
                arrImage[2] = imageThumbnail3;
                imageThumbnail3.Visibility = Visibility.Visible;
                imageThumbnail1.SetValue(Grid.RowSpanProperty, 1);
                imageThumbnail2.SetValue(Grid.RowSpanProperty, 1);
            }

            if (arrPathname.Count >= 4)
            {
                arrImage[3] = imageThumbnail4;
                imageThumbnail4.Visibility = Visibility.Visible;
            }

            int idx = 0;
            foreach (string pathname in arrPathname)
            {
                if (idx >= 4)
                    break;

                arrImage[idx].Source = this.GetImageStream(pathname);

                BitmapImage bitmapImage = (BitmapImage)arrImage[idx].Source;
                arrImage[idx].Stretch = Stretch.Uniform;
                idx++;
            }

            txtDownloadLinks.Text = dispinfoSelectBjData.DownloadLink;
        }

        private void dgridJav2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dispinfoSelectJav2 = (Jav2Data)dgridJav2.SelectedItem;

            if (dispinfoSelectJav2 == null)
                return;

            txtJav2DownloadLinks.Text = common.DonwloadLinks.GetTargetOnly(dispinfoSelectJav2.DownloadLinks);
            txtJav2Detail.Text = dispinfoSelectJav2.FilesInfo;
        }

        private void btnJav2Search_Click(object sender, RoutedEventArgs e)
        {
            colJav2.SetSearchText(txtJav2Search.Text);
            colJav2.Execute();
        }

        private void btnCopyTextJav_Click(object sender, RoutedEventArgs e)
        {
            UpdateJavDownloadFiles(txtDownloadLinks.Text);

            CopyClipboard(txtDownloadLinks.Text);
        }

        private void btnCopyText_Click(object sender, RoutedEventArgs e)
        {
            string TargetFiles = "";
            if (txtJav2DownloadLinks.Text.IndexOf("https://btafile.com/", StringComparison.Ordinal) >= 0)
            {
                string[] arr_btafile = txtJav2Detail.Text.Split('、');
                foreach (string fileandsize in arr_btafile)
                {
                    string filename = Regex.Replace(fileandsize, "\\s.*", "");
                    if (TargetFiles.Length > 0)
                        TargetFiles = TargetFiles + "、" + filename.Trim();
                    else
                        TargetFiles = filename.Trim();
                }
            }
            else
            {
                TargetFiles = txtJav2DownloadLinks.Text;
            }
            UpdateJavDownloadFiles(TargetFiles);

            CopyClipboard(txtJav2DownloadLinks.Text);
        }

        /// <summary>
        /// scraping.jav.download_filesの列を更新する
        /// </summary>
        /// <param name="myDownloadLinks"></param>
        private void UpdateJavDownloadFiles(string myDownloadLinks)
        {
            string filesText = common.DonwloadLinks.GetFilesText(myDownloadLinks);
            javDao.UpdateDownloadFiles(dispinfoSelectJavData, filesText);
        }

        private void CopyClipboard(string myText)
        {
            if (!String.IsNullOrEmpty(myText))
            {
                try
                {
                    Clipboard.SetData(DataFormats.Text, myText);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return;
                }
            }
        }

        private void btnSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (dispctrlDataGrid == DATAGRID_JAV)
                dispctrlDataGrid = DATAGRID_BJ;
            else
                dispctrlDataGrid = DATAGRID_JAV;

            SwitchDataGrid(dispctrlDataGrid);
        }

        private void btnSearchJav_Click(object sender, RoutedEventArgs e)
        {
            if (dispctrlDataGrid == DATAGRID_JAV)
            {
                colJav.SetSearchText(txtSearchJav.Text);
                colJav.Execute();
            }
            else
            {
                colBj.SetSearchText(txtSearchJav.Text);
                colBj.Execute();
            }
        }

        private void btnSearchJavCancel_Click(object sender, RoutedEventArgs e)
        {
            if (dispctrlDataGrid == DATAGRID_JAV)
            {
                colJav.SetSearchText("");
                colJav.Execute();
            }
            else
            {
                colBj.SetSearchText("");
                colBj.Execute();
            }
        }

        private void OnClickFilterSelection(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button == null)
                return;

            string contentButton = button.Content.ToString();

            int isSelection = -9;
            if (contentButton.Equals("未"))
                isSelection = 0;
            else if (contentButton.Equals("対象"))
                isSelection = IS_SELECTION_TARGET;
            else if (contentButton.Equals("HD"))
                isSelection = IS_SELECTION_WAITING_HD;
            else if (contentButton.Equals("済"))
                isSelection = IS_SELECTION_DONE;

            if (dispctrlDataGrid == DATAGRID_JAV)
            {
                colJav.SetSearchSelection(isSelection);
                colJav.Execute();
            }
            else
            {
                colBj.SetSearchSelection(isSelection);
                colBj.Execute();
            }
        }

        private void dgridFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FileInfo fileInfo = (FileInfo)dgridFiles.SelectedItem;

            if (fileInfo == null)
                return;

            Process.Start(fileInfo.FullName);
        }

        private void btnUpdateJav_Click(object sender, RoutedEventArgs e)
        {
            dispinfoSelectJavData.ProductNumber = txtRegisterProductNumber.Text;
            dispinfoSelectJavData.Actress = txtRegisterActress.Text;
            dispinfoSelectJavData.SellDate = Convert.ToDateTime(txtRegisterSellDate.Text);
            dispinfoSelectJavData.Package = txtRegisterPackage.Text;
            dispinfoSelectJavData.Thumbnail = txtRegisterThumbnail.Text;
            dispinfoSelectJavData.DownloadFiles = txtRegisterDownloadFiles.Text;

            javDao.Update(dispinfoSelectJavData);
        }

        private void OnChangedRating(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;

            int changeRating = Convert.ToInt32(combo.SelectedItem);
            int beforeRating = 0;

            if (dispinfoSelectJavData == null)
                return;

            beforeRating = dispinfoSelectJavData.Rating;

            if (changeRating == beforeRating)
                return;

            dispinfoSelectJavData.Rating = changeRating;
            javDao.UpdateRating(dispinfoSelectJavData);
        }

        private void MenuItemDgridJav2UrlCopy_Click(object sender, RoutedEventArgs e)
        {
            Jav2Data data = (Jav2Data)dgridJav2.SelectedItem;

            CopyClipboard(data.Url);
        }

        private void btnJav2Url_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(dispinfoSelectJav2.Url);
        }

        private void btnJumpUrl_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(dispinfoSelectJavData.Url);
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();

            try
            {
                if (data.GetDataPresent(DataFormats.Text))
                {
                    string ClipboardText = (string)data.GetData(DataFormats.Text);
                    // クリップボードのテキストを改行毎に配列に設定
                    char[] separator = new char[] { '\r', '\n' };
                    string[] ClipBoardList = ClipboardText.Split(separator);

                    txtRegisterDownloadFiles.Text = string.Join(" ", ClipBoardList);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnBeforeThumbnail_Click(object sender, RoutedEventArgs e)
        {
            if (dispinfoThumbnailsArray == null)
                return;

            if (dispinfoThumbnailsArray.Length <= 1)
                return;

            if (dispinfoCurrentThumbnail == 0)
                return;

            string filname = "";

            if (dispinfoThumbnailsArray.Length > 1)
            {
                dispinfoCurrentThumbnail = dispinfoCurrentThumbnail - 1;
                if (dispinfoSelectJavData.Thumbnail.IndexOf("http", StringComparison.Ordinal) == 0)
                    filname = System.IO.Path.GetFileName(dispinfoThumbnailsArray[dispinfoCurrentThumbnail]);
                else
                    filname = dispinfoThumbnailsArray[dispinfoCurrentThumbnail];

            }

            string pathname = System.IO.Path.Combine(ImagePath, filname);

            if (System.IO.File.Exists(pathname))
            {
                imageThumbnail.Source = this.GetImageStream(pathname);

                if (imageThumbnail.Source != null)
                {
                    BitmapImage bitmapImage = (BitmapImage)imageThumbnail.Source;
                    imageThumbnail.Width = lgridMain.ColumnDefinitions[1].ActualWidth;
                    imageThumbnail.Height = (imageThumbnail.Width / bitmapImage.Width) * bitmapImage.Height;
                    imageThumbnail.Stretch = Stretch.Uniform;
                }
            }
        }

        private void btnNextThumbnail_Click(object sender, RoutedEventArgs e)
        {
            if (dispinfoThumbnailsArray == null)
                return;

            if (dispinfoThumbnailsArray.Length <= 1)
                return;

            if (dispinfoCurrentThumbnail + 1 >= dispinfoThumbnailsArray.Length)
                return;

            string filname = "";

            if (dispinfoThumbnailsArray.Length > 1)
            {
                dispinfoCurrentThumbnail = dispinfoCurrentThumbnail + 1;
                if (dispinfoSelectJavData.Thumbnail.IndexOf("http", StringComparison.Ordinal) == 0)
                    filname = Path.GetFileName(dispinfoThumbnailsArray[dispinfoCurrentThumbnail]);
                else
                    filname = dispinfoThumbnailsArray[dispinfoCurrentThumbnail];

            }

            string pathname = Path.Combine(ImagePath, filname);

            if (File.Exists(pathname))
            {
                imageThumbnail.Source = this.GetImageStream(pathname);

                if (imageThumbnail.Source != null)
                {
                    BitmapImage bitmapImage = (BitmapImage)imageThumbnail.Source;
                    imageThumbnail.Width = lgridMain.ColumnDefinitions[1].ActualWidth;
                    imageThumbnail.Height = (imageThumbnail.Width / bitmapImage.Width) * bitmapImage.Height;
                    imageThumbnail.Stretch = Stretch.Uniform;
                }
            }
        }
    }
}
