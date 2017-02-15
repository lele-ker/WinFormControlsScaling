using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace WinFormControlScaling
{

    /// <summary>
    /// 依照視窗大小自動縮放內部的控件,容器控件支援 Panel,GroupBox,TabControl,FlowLayoutPanel,TableLayoutPanel,SplitContainer
    /// </summary>
    public class WinFormControlScaling
    {

        private ControlInfo formInfo;
        private List<List<ControlInfo>> controlsRecord;
        private Font autoSizeFont;



        /// <summary>
        /// 建立Form底下所有控件位置與大小的資訊
        /// </summary>
        /// <param name="Form">表單物件(Form)</param>
        public WinFormControlScaling(Form Form)
        {

            UpdateFormInfo(ref Form);
            DecideFontSizeByFormSize(ref Form);
            controlsRecord = new List<List<ControlInfo>>();


            foreach (Control ct in Form.Controls)
            {
                List<ControlInfo> cirs = new List<ControlInfo>();
                UpdateControlsRecord(ref cirs, ct);
                controlsRecord.Add(cirs);
            }

        }

        /// <summary>
        /// 更新 FormRecord 資料
        /// </summary>
        /// <param name="Form">表單物件(Form)</param>
        private void UpdateFormInfo(ref Form Form)
        {

            if (formInfo == null)
            {
                formInfo = new ControlInfo();
            }


            formInfo.Name = Form.Name;
            formInfo.Top = Form.Top;
            formInfo.Left = Form.Left;
            formInfo.Width = Form.Width;
            formInfo.Height = Form.Height;

        }

        /// <summary>
        /// 依據視窗目前大小決定控制取得字型大小設定
        /// </summary>
        /// <param name="Form">表單物件(Form)</param>
        private void DecideFontSizeByFormSize(ref Form Form)
        {

            int width = Form.Width;


            //檢查順序要由小到大
            if (width < 1280)
            {
                autoSizeFont = new Font(Form.Font.FontFamily, 8);
            }
            else if (width <= 1920)
            {
                autoSizeFont = new Font(Form.Font.FontFamily, 12);
            }
            else if (width > 1920 && width < 2300)
            {
                autoSizeFont = new Font(Form.Font.FontFamily, 18);
            }
            else if (width >= 2300)
            {
                autoSizeFont = new Font(Form.Font.FontFamily, 25);
            }

        }

        /// <summary>
        /// 紀錄所有 Form底下的 Control 物件的 位置與大小
        /// </summary>
        /// <param name="Controls">控制項資訊物件(ControlInfoRecord)List</param>
        /// <param name="Ctrl">控制項物件(Control)</param>
        private void UpdateControlsRecord(ref List<ControlInfo> Controls, Control Ctrl)
        {

            Controls.Add(UpdateSingleControlRecord(Ctrl));


            if (Ctrl is TabControl)
            {
                SearchControlsInTabControl(ref Controls, ref Ctrl);
            }
            else
            {
                SearchControlsInContentControl(ref Controls, ref Ctrl);
            }

        }

        /// <summary>
        /// 儲存control物件的資料到 ControlInfoRecord 並回傳
        /// </summary>
        /// <param name="Ctrl">控制項物件(Control)</param>
        private ControlInfo UpdateSingleControlRecord(Control Ctrl)
        {

            ControlInfo cinfo = new ControlInfo();


            cinfo.Name = Ctrl.Name;
            cinfo.Top = Ctrl.Top;
            cinfo.Left = Ctrl.Left;
            cinfo.Width = Ctrl.Width;
            cinfo.Height = Ctrl.Height;


            return cinfo;

        }

        /// <summary>
        /// 搜尋 TabControl 底下的所有Controls 並記錄到 List<ControlInfoRecord> CIRs 內
        /// </summary>
        /// <param name="Controls">控制項資訊物件(ControlInfoRecord)List</param>
        /// <param name="Ctrl">控制項物件(Control)</param>
        private void SearchControlsInTabControl(ref List<ControlInfo> Controls, ref Control Ctrl)
        {

            TabControl tc = Ctrl as TabControl;


            if (tc.TabPages.Count.Equals(0))
            {
                return;
            }


            foreach (TabPage tp in tc.TabPages)
            {
                foreach (Control tpcr in tp.Controls)
                {
                    UpdateControlsRecord(ref Controls, tpcr);
                }
            }

        }

        /// <summary>
        /// 搜尋 控件 底下的所有Controls 並記錄到 List<ControlInfoRecord> CIRs 內
        /// </summary>
        /// <param name="Controls">控制項資訊物件(ControlInfoRecord)List</param>
        /// <param name="Ctrl">控制項物件(Control)</param>
        private void SearchControlsInContentControl(ref List<ControlInfo> Controls, ref Control Ctrl)
        {

            foreach (Control ctcr in Ctrl.Controls)
            {
                UpdateControlsRecord(ref Controls, ctcr);
            }

        }



        /// <summary>
        /// 調整所有控件的大小
        /// </summary>
        /// <param name="Form">表單物件(Form)</param>
        public void Scale(Form Form)
        {

            float widthScale;
            float heightScale;
            int recordCount;
            int detailCount;


            DecideFontSizeByFormSize(ref Form);
            widthScale = (float)Form.Width / (float)formInfo.Width;
            heightScale = (float)Form.Height / (float)formInfo.Height;
            recordCount = 0;


            foreach (Control ct in Form.Controls)
            {
                detailCount = 0;
                ScaleControls(ref widthScale, ref heightScale, ref recordCount, ref detailCount, ct);
                recordCount++;
            }

        }

        /// <summary>
        /// 縮放所有控制項物件的大小與位置
        /// </summary>
        /// <param name="WidthScale">視窗寬度縮放比例</param>
        /// <param name="HeightScale">視窗高度縮放比例</param>
        /// <param name="RecordCount">二維ControlInfoRecord資料的最上層位置索引</param>
        /// <param name="DetailCount">二維ControlInfoRecord資料的第二層位置索引</param>
        /// <param name="Ctrl">控制項物件(Control)</param>
        private void ScaleControls(ref float WidthScale, ref float HeightScale, ref int RecordCount, ref int DetailCount, Control Ctrl)
        {

            ScaleControl(ref WidthScale, ref HeightScale, ref RecordCount, ref DetailCount, Ctrl);


            if (Ctrl is TabControl)
            {
                ScaleTabControl(ref WidthScale, ref HeightScale, ref RecordCount, ref DetailCount, ref Ctrl); 
            }
            else if (Ctrl is MenuStrip)
            {
                ScaleMenuStrip(ref Ctrl);
            }
            else 
            {
                ScaleSubLevelControl(ref WidthScale, ref HeightScale, ref RecordCount, ref DetailCount, ref Ctrl);
            }

        }

        /// <summary>
        /// 執行單一控制項物件的縮放
        /// </summary>
        /// <param name="WidthScale">視窗寬度縮放比例</param>
        /// <param name="HeightScale">視窗高度縮放比例</param>
        /// <param name="RecordCount">二維ControlInfoRecord資料的最上層位置索引</param>
        /// <param name="DetailCount">二維ControlInfoRecord資料的第二層位置索引</param>
        /// <param name="Ctrl">控制項物件(Control)</param>
        private void ScaleControl(ref float WidthScale, ref float HeightScale, ref int RecordCount, ref int DetailCount, Control Ctrl)
        {

            ControlInfo cir = controlsRecord[RecordCount][DetailCount];


            Ctrl.Top = Convert.ToInt32(cir.Top * HeightScale);
            Ctrl.Height = Convert.ToInt32(cir.Height * HeightScale);
            Ctrl.Left = Convert.ToInt32(cir.Left * WidthScale);
            Ctrl.Width = Convert.ToInt32(cir.Width * WidthScale);
            Ctrl.Font = autoSizeFont;


            DetailCount++;

        }

        /// <summary>
        /// 搜尋 TabControl 底下的所有Controls 並執行縮放
        /// </summary>
        /// <param name="WidthScale">視窗寬度縮放比例</param>
        /// <param name="HeightScale">視窗高度縮放比例</param>
        /// <param name="RecordCount">二維ControlInfoRecord資料的最上層位置索引</param>
        /// <param name="DetailCount">二維ControlInfoRecord資料的第二層位置索引</param>
        /// <param name="Ctrl">控制項物件(Control)</param>
        private void ScaleTabControl(ref float WidthScale, ref float HeightScale, ref int RecordCount, ref int DetailCount, ref Control Ctrl)
        {

            TabControl tc = Ctrl as TabControl;


            if (tc.TabPages.Count.Equals(0))
            {
                return;
            }


            foreach (TabPage tp in tc.TabPages)
            {
                foreach (Control tpcr in tp.Controls)
                {
                    ScaleControls(ref WidthScale, ref HeightScale, ref RecordCount, ref DetailCount, tpcr);
                }
            }

        }

        /// <summary>
        /// 搜尋 MenuStrip 底下的所有 items 並執行字型大小縮放
        /// </summary>
        /// <param name="WidthScale">視窗寬度縮放比例</param>
        /// <param name="HeightScale">視窗高度縮放比例</param>
        /// <param name="RecordCount">二維ControlInfoRecord資料的最上層位置索引</param>
        /// <param name="DetailCount">二維ControlInfoRecord資料的第二層位置索引</param>
        /// <param name="Ct">控制項物件(Control)</param>
        private void ScaleMenuStrip(ref Control Ct)
        {

            MenuStrip ms = Ct as MenuStrip;


            if (ms.Items.Count.Equals(0))
            {
                return;
            }


            foreach (ToolStripItem tsi in ms.Items)
            {
                tsi.Font = autoSizeFont;
            }

        }

        /// <summary>
        /// 搜尋 控件 底下的所有Controls 並執行縮放
        /// </summary>
        /// <param name="WidthScale">視窗寬度縮放比例</param>
        /// <param name="HeightScale">視窗高度縮放比例</param>
        /// <param name="RecordCount">二維ControlInfoRecord資料的最上層位置索引</param>
        /// <param name="DetailCount">二維ControlInfoRecord資料的第二層位置索引</param>
        /// <param name="Ct">控制項物件(Control)</param>
        private void ScaleSubLevelControl(ref float WidthScale, ref float HeightScale, ref int RecordCount, ref int DetailCount, ref Control Ct)
        {

            foreach (Control tpcr in Ct.Controls)
            {
                ScaleControls(ref WidthScale, ref HeightScale, ref RecordCount, ref DetailCount, tpcr);
            }
        }



        /// <summary>
        /// 取得表單物件初始的長寬與位置的資料
        /// </summary>
        public ControlInfo GetFormRecord()
        {
            return formInfo;
        }
        /// <summary>
        /// 取得表單底下所有控制項的初始的長寬與位置的資料
        /// </summary>
        public List<List<ControlInfo>> GetControlsRecord()
        {
            return controlsRecord;
        }


    }


    /// <summary>
    /// 紀錄control的位置與大小的物件
    /// </summary>
    public class ControlInfo
    {
        public string Name;
        public int Top = 0;
        public int Left = 0;
        public int Width = 0;
        public int Height = 0;
    }

    /// <summary>
    /// 儲存scale控件大小時，各種Form寬度大小對應的字型大小的Context資訊
    /// </summary>
    public class FontSizeInfoPackage
    {

        private List<FontSizeInfo> fontSizeInfo;


        public FontSizeInfoPackage()
        {
            fontSizeInfo = new List<FontSizeInfo>();
        }
             
        /// <summary>
        /// 清除全部已存的 FontSizeInfo 資料
        /// </summary>
        public void Clear()
        {
            fontSizeInfo.Clear();
        }

        /// <summary>
        /// 存入Context資訊，如果已經有相同的Form寬度的Context資訊的話，就不會存入
        /// </summary>
        /// <param name="WinFormWidth">Form寬度</param>
        /// <param name="FontSize">字型大小</param>
        public void Add(FontSizeInfo[] Infos)
        {
            foreach (FontSizeInfo info in Infos)
            {
                Add(info);
            }
        }
        /// <summary>
        /// 存入Context資訊，如果已經有相同的Form寬度的Context資訊的話，就不會存入
        /// </summary>
        /// <param name="WinFormWidth">Form寬度</param>
        /// <param name="FontSize">字型大小</param>
        public void Add(FontSizeInfo Info)
        {
            Add(Info.Width, Info.FontSize);  
        }
        /// <summary>
        /// 存入Context資訊，如果已經有相同的Form寬度的Context資訊的話，就不會存入
        /// </summary>
        /// <param name="WinFormWidth">Form寬度</param>
        /// <param name="FontSize">字型大小</param>
        public void Add(int WinFormWidth, int FontSize)
        {

            if ((fontSizeInfo != null) && (fontSizeInfo.Count > 0))
            {
                foreach (FontSizeInfo info in fontSizeInfo)
                {
                    if (info.Width.Equals(WinFormWidth))
                    {
                        return;
                    }
                }
            }

            fontSizeInfo.Add(new FontSizeInfo(WinFormWidth, FontSize));

        }

        /// <summary>
        /// 取得全部Context對應的字型大小的資訊
        /// </summary>
        public FontSizeInfo[] FontSizeInfo
        {
            get
            {
                fontSizeInfo.Sort((x, y) => x.Width.CompareTo(y.Width));
                return fontSizeInfo.ToArray();
            }
        }

    }
    
    /// <summary>
    /// 各個WinForm寬度情況下的字型資訊
    /// </summary>
    public class FontSizeInfo
    {
        /// <summary>
        /// WinForm寬度
        /// </summary>
        public int Width;
        /// <summary>
        /// 字型大小
        /// </summary>
        public int FontSize;

        /// <summary>
        /// 各個WinForm寬度情況下的字型資訊的建構函式
        /// </summary>
        /// <param name="Width">WinForm寬度</param>
        /// <param name="FontSize">字型大小</param>
        public FontSizeInfo(int Width, int FontSize)
        {
            this.Width = Width;
            this.FontSize = FontSize;
        }
    }


}
