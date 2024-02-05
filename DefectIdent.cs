using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using TimeTech.Core;
using TimeTech.Core.Common;
using TimeTech.Core.DataProvider;
using TimeTech.DeviceDLL.OPR;

namespace PerovskiteTest
{
    /// <summary>
    /// 易渊AI缺陷识别类
    /// </summary>
    public class DefectIdent
    {
        private Bitmap m_bitmapSrc = null;
        private Bitmap m_bitmapDst = null;
        private IntPtr m_model = IntPtr.Zero;
        private List<Rectangle> m_liRect = new List<Rectangle>();
        HalconProvider halconProvider = new TimeTech.Core.Common.HalconProvider();
        bool UseGPU
        {
            get
            {
                bool blTemp = false;
                bool.TryParse(ConfigurationManager.AppSettings["UseGPU"], out blTemp);
                return blTemp;
            }
        }
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="modelFileFullPath">模型全路径</param>
        public DefectIdent(string modelFileFullPath)
        {
            LoadModel(modelFileFullPath);
        }

        public DefectIdent()
        {
        }
        /// <summary>
        /// 加载图像识别模型
        /// </summary>
        /// <param name="modelFileFullPath"></param>
        /// <returns></returns>
        public bool LoadModel(string modelFileFullPath)
        {
            try
            {
                // Clear
                if (m_model != IntPtr.Zero)
                {
                    DPInfer.DnnInfer_Close(m_model);
                    m_model = IntPtr.Zero;
                }

                // Initialize model
                if (UseGPU)
                    m_model = DPInfer.DnnInfer_Init(modelFileFullPath, 1, 1, 0);     // GPU mode
                else
                    m_model = DPInfer.DnnInfer_Init(modelFileFullPath, 1, 0, 0);     // CPU mode
                if (m_model != IntPtr.Zero)
                {
                    int iModelType = DPInfer.DnnInfer_GetModelType(m_model);
                    string strModelType = "Unknown";
                    if (iModelType == 0)
                        strModelType = "Classify";
                    else if (iModelType == 1)
                        strModelType = "ObjectDetect";
                    else if (iModelType == 2)
                        strModelType = "Segment";
                    strModelType = "ModelType:" + strModelType;
                    //MessageBox.Show("Load Model Success", "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                else
                {
                    //MessageBox.Show("Load Model Failed", "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            catch (Exception err) { FileLog.AddErrorLog("DefectIdent.LoadModel 函数运行出错", err); return false; }
        }

        /// <summary>
        /// 获取缺陷识别结果
        /// </summary>
        /// <param name="imageFullPath">图片全路径</param>
        /// <param name="bitmap">传入原图，输出缺陷图</param>
        /// <returns>返回缺陷内容列表</returns>
        public List<string> GetDefectIdentResult(ref Bitmap bitmap)
        {
            List<string> strInfo = new List<string>();
            m_bitmapSrc = bitmap;
            Debug.Assert(m_model != IntPtr.Zero && m_bitmapSrc != null);

            m_liRect.Clear();
            //pbxSrc.Refresh();

            // Create datum
            DPShape sIn = DPInfer.Shape_CreateV2(1, 3, (uint)m_bitmapSrc.Height, (uint)m_bitmapSrc.Width);  // Channel = 3 for BGR24Bit image
            IntPtr pDatumIn = DPInfer.Datum_CreateV2(1);
            DPInfer.Datum_Reshape(pDatumIn, sIn);

            // Fill input image data
            BitmapData dataSrc = m_bitmapSrc.LockBits(new Rectangle(0, 0, m_bitmapSrc.Width, m_bitmapSrc.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            IntPtr pDataDst = DPInfer.Datum_GetData(pDatumIn);
            for (int i = 0; i < m_bitmapSrc.Height; i++)
                DPInfer.MemCopy((IntPtr)(pDataDst.ToInt64() + m_bitmapSrc.Width * 3 * i), (IntPtr)(dataSrc.Scan0.ToInt64() + dataSrc.Stride * i), (uint)m_bitmapSrc.Width * 3);
            m_bitmapSrc.UnlockBits(dataSrc);

            // Infer
            int iModelType = DPInfer.DnnInfer_GetModelType(m_model);
            if (iModelType == 0)        // Classify
            {
                int iLabelIndex = -1;
                int iClassNum = DPInfer.DnnInfer_GetClassNum(m_model);
                float[] fProbs = new float[iClassNum];
                if (true == DPInfer.DnnInfer_Cls_Infer(m_model, pDatumIn, ref iLabelIndex, fProbs))
                {
                    //strInfo = "Classify Infer Success";
                    if (iLabelIndex != -1)
                    {
                        StringBuilder sbName = new StringBuilder(16);
                        DPInfer.DnnInfer_GetLabelName(m_model, sbName, iLabelIndex);
                        //strInfo += " (" + sbName.ToString() + ":" + fProbs[iLabelIndex].ToString("F2") + ")";
                        strInfo.Add(" (" + sbName.ToString() + ":" + fProbs[iLabelIndex].ToString("F2") + ")");
                    }
                    //MessageBox.Show(strInfo, "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                }
                //MessageBox.Show("Classify Infer Failed", "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else if (iModelType == 1)   // Object detect
            {
                int iUseSize = 0;
                DPDetResult[] detResults = new DPDetResult[10240];
                if (true == DPInfer.DnnInfer_Det_Infer(m_model, pDatumIn, detResults, 10240, ref iUseSize))
                {
                    //strInfo = "ObjectDetect Infer Success";
                    for (int i = 0; i < iUseSize; i++)
                    {
                        m_liRect.Add(new Rectangle(detResults[i].left, detResults[i].top, detResults[i].width, detResults[i].height));
                        //strInfo += "\n" + i + ",L=" + detResults[i].left + ",T=" + detResults[i].top + ",W=" + detResults[i].width + ",H=" + detResults[i].height + " (" + detResults[i].name + ":" + detResults[i].score.ToString("F2") + ")";

                        strInfo.Add("L=" + detResults[i].left + ",T=" + detResults[i].top + ",W=" + detResults[i].width + ",H=" + detResults[i].height + "|(" + detResults[i].name + ":" + detResults[i].score.ToString("F2") + ")");
                    }
                    //pbxSrc.Refresh();
                    //MessageBox.Show(strInfo, "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    //MessageBox.Show("ObjectDetect Infer Failed", "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else if (iModelType == 2)   // Segment
            {
                DPShape sOut = DPInfer.Shape_CreateV2(1, 1, (uint)m_bitmapSrc.Height, (uint)m_bitmapSrc.Width);  // Channel = 1 for Gray image
                IntPtr pDatumOut = DPInfer.Datum_CreateV2(1);
                DPInfer.Datum_Reshape(pDatumOut, sOut);

                if (true == DPInfer.DnnInfer_Seg_Infer(m_model, pDatumIn, pDatumOut))
                {
                    // Fill output image data
                    if (m_bitmapDst != null)
                    {
                        m_bitmapDst.Dispose();
                        m_bitmapDst = null;
                    }
                    m_bitmapDst = new Bitmap(m_bitmapSrc.Width, m_bitmapSrc.Height, PixelFormat.Format8bppIndexed);
                    BitmapData dataTemp = m_bitmapDst.LockBits(new Rectangle(0, 0, m_bitmapDst.Width, m_bitmapDst.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                    IntPtr pDataTemp = DPInfer.Datum_GetData(pDatumOut);
                    for (int i = 0; i < m_bitmapDst.Height; i++)
                        DPInfer.MemCopy((IntPtr)(dataTemp.Scan0.ToInt64() + dataTemp.Stride * i), (IntPtr)(pDataTemp.ToInt64() + m_bitmapDst.Width * i), (uint)m_bitmapDst.Width);
                    m_bitmapDst.UnlockBits(dataTemp);
                    bitmap = m_bitmapDst;
                    int iUseSize = 0;
                    DPSegRectResult[] segRectResults = new DPSegRectResult[4048];
                    DPInfer.DnnInfer_Seg_Label2Rect(m_model, pDatumOut, segRectResults, 2048, ref iUseSize);
                    //strInfo = "Segment Infer Success";
                    for (int i = 0; i < iUseSize; i++)
                    {
                        m_liRect.Add(new Rectangle(segRectResults[i].rect_left, segRectResults[i].rect_top, segRectResults[i].rect_width, segRectResults[i].rect_height));
                        StringBuilder sbName = new StringBuilder(16);
                        DPInfer.DnnInfer_GetLabelName(m_model, sbName, segRectResults[i].label);
                        //strInfo += "\n" + i + ",L=" + segRectResults[i].rect_left + ",T=" + segRectResults[i].rect_top + ",W=" + segRectResults[i].rect_width + ",H=" + segRectResults[i].rect_height + " (" + sbName.ToString() + ")";
                        strInfo.Add("L=" + segRectResults[i].rect_left + ",T=" + segRectResults[i].rect_top + ",W=" + segRectResults[i].rect_width + ",H=" + segRectResults[i].rect_height + "|(" + sbName.ToString().Replace("|", " ") + ")");
                    }
                    //pbxSrc.Refresh();
                    //MessageBox.Show(strInfo, "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    //MessageBox.Show("Segment Infer Failed", "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    DPInfer.Datum_Destroy(pDatumOut);
            }

            // Release datum
            DPInfer.Datum_Destroy(pDatumIn);
            return strInfo;
        }

        /// <summary>
        /// 获取缺陷识别结果
        /// </summary>
        /// <param name="bitmap">传入原图，输出缺陷图</param>
        /// <param name="list">输出缺陷数据集</param>
        /// <returns></returns>
        public List<string> GetDefectIdentResultForSiC(ref Bitmap bitmap, ref List<Halcon_DefectIdentInfo> list)
        {
            List<string> strInfo = new List<string>();
            m_bitmapSrc = bitmap;
            Debug.Assert(m_model != IntPtr.Zero && m_bitmapSrc != null);

            m_liRect.Clear();
            //pbxSrc.Refresh();

            // Create datum
            DPShape sIn = DPInfer.Shape_CreateV2(1, 3, (uint)m_bitmapSrc.Height, (uint)m_bitmapSrc.Width);  // Channel = 3 for BGR24Bit image
            IntPtr pDatumIn = DPInfer.Datum_CreateV2(1);
            DPInfer.Datum_Reshape(pDatumIn, sIn);

            // Fill input image data
            BitmapData dataSrc = m_bitmapSrc.LockBits(new Rectangle(0, 0, m_bitmapSrc.Width, m_bitmapSrc.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            IntPtr pDataDst = DPInfer.Datum_GetData(pDatumIn);
            for (int i = 0; i < m_bitmapSrc.Height; i++)
                DPInfer.MemCopy((IntPtr)(pDataDst.ToInt64() + m_bitmapSrc.Width * 3 * i), (IntPtr)(dataSrc.Scan0.ToInt64() + dataSrc.Stride * i), (uint)m_bitmapSrc.Width * 3);
            m_bitmapSrc.UnlockBits(dataSrc);

            // Infer
            int iModelType = DPInfer.DnnInfer_GetModelType(m_model);
            if (iModelType == 0)        // Classify
            {
                int iLabelIndex = -1;
                int iClassNum = DPInfer.DnnInfer_GetClassNum(m_model);
                float[] fProbs = new float[iClassNum];
                if (true == DPInfer.DnnInfer_Cls_Infer(m_model, pDatumIn, ref iLabelIndex, fProbs))
                {
                    //strInfo = "Classify Infer Success";
                    if (iLabelIndex != -1)
                    {
                        StringBuilder sbName = new StringBuilder(16);
                        DPInfer.DnnInfer_GetLabelName(m_model, sbName, iLabelIndex);
                        //strInfo += " (" + sbName.ToString() + ":" + fProbs[iLabelIndex].ToString("F2") + ")";
                        strInfo.Add(" (" + sbName.ToString() + ":" + fProbs[iLabelIndex].ToString("F2") + ")");
                    }
                    //MessageBox.Show(strInfo, "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                }
                //MessageBox.Show("Classify Infer Failed", "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else if (iModelType == 1)   // Object detect
            {
                int iUseSize = 0;
                DPDetResult[] detResults = new DPDetResult[128];
                if (true == DPInfer.DnnInfer_Det_Infer(m_model, pDatumIn, detResults, 128, ref iUseSize))
                {
                    //strInfo = "ObjectDetect Infer Success";
                    for (int i = 0; i < iUseSize; i++)
                    {
                        m_liRect.Add(new Rectangle(detResults[i].left, detResults[i].top, detResults[i].width, detResults[i].height));
                        //strInfo += "\n" + i + ",L=" + detResults[i].left + ",T=" + detResults[i].top + ",W=" + detResults[i].width + ",H=" + detResults[i].height + " (" + detResults[i].name + ":" + detResults[i].score.ToString("F2") + ")";

                        strInfo.Add("L=" + detResults[i].left + ",T=" + detResults[i].top + ",W=" + detResults[i].width + ",H=" + detResults[i].height + "|(" + detResults[i].name + ":" + detResults[i].score.ToString("F2") + ")");
                    }
                    //pbxSrc.Refresh();
                    //MessageBox.Show(strInfo, "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    //MessageBox.Show("ObjectDetect Infer Failed", "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else if (iModelType == 2)   // Segment
            {
                DPShape sOut = DPInfer.Shape_CreateV2(1, 1, (uint)m_bitmapSrc.Height, (uint)m_bitmapSrc.Width);  // Channel = 1 for Gray image
                IntPtr pDatumOut = DPInfer.Datum_CreateV2(1);
                DPInfer.Datum_Reshape(pDatumOut, sOut);

                if (true == DPInfer.DnnInfer_Seg_Infer(m_model, pDatumIn, pDatumOut))
                {
                    // Fill output image data
                    if (m_bitmapDst != null)
                    {
                        m_bitmapDst.Dispose();
                        m_bitmapDst = null;
                    }
                    m_bitmapDst = new Bitmap(m_bitmapSrc.Width, m_bitmapSrc.Height, PixelFormat.Format8bppIndexed);
                    #region 先将创建的Bitmap通过Halcon转换成单通道灰度图
                    HObject ho_Image, ho_Image1;
                    // Initialize local and output iconic variables 
                    HOperatorSet.GenEmptyObj(out ho_Image);
                    HOperatorSet.GenEmptyObj(out ho_Image1);
                    ho_Image.Dispose();
                    ho_Image1.Dispose();
                    halconProvider.Bitmap2HObjectBpp8(m_bitmapDst, out ho_Image);
                    halconProvider.HObject2Bpp8_(ho_Image, out m_bitmapDst);
                    //bitmap = m_bitmapDst;
                    ho_Image.Dispose();
                    #endregion
                    BitmapData dataTemp = m_bitmapDst.LockBits(new Rectangle(0, 0, m_bitmapDst.Width, m_bitmapDst.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                    IntPtr pDataTemp = DPInfer.Datum_GetData(pDatumOut);
                    for (int i = 0; i < m_bitmapDst.Height; i++)
                        DPInfer.MemCopy((IntPtr)(dataTemp.Scan0.ToInt64() + dataTemp.Stride * i), (IntPtr)(pDataTemp.ToInt64() + m_bitmapDst.Width * i), (uint)m_bitmapDst.Width);
                    m_bitmapDst.UnlockBits(dataTemp);
                    bitmap = m_bitmapDst;
                    //bitmap.Save("D:\\Users\\lv101\\微信文件\\WeChat Files\\wxid_t6sov1p418ka22\\FileStorage\\File\\2023-10\\33.png");
                    //pbxDst.Image = m_bitmapDst;
                    //pbxDst.SizeMode = PictureBoxSizeMode.Zoom;
                    /*
                    list = GetDefectIdentByHalcon(m_bitmapDst);
                    // Parse object information
                    int iUseSize = 0;
                    DPSegRectResult[] segRectResults = new DPSegRectResult[10240];
                    DPInfer.DnnInfer_Seg_Label2Rect(m_model, pDatumOut, segRectResults, 10240, ref iUseSize);
                    //strInfo = "Segment Infer Success";
                    for (int i = 0; i < iUseSize; i++)
                    {
                        m_liRect.Add(new Rectangle(segRectResults[i].rect_left, segRectResults[i].rect_top, segRectResults[i].rect_width, segRectResults[i].rect_height));
                        StringBuilder sbName = new StringBuilder(16);
                        DPInfer.DnnInfer_GetLabelName(m_model, sbName, segRectResults[i].label);
                        //strInfo += "\n" + i + ",L=" + segRectResults[i].rect_left + ",T=" + segRectResults[i].rect_top + ",W=" + segRectResults[i].rect_width + ",H=" + segRectResults[i].rect_height + " (" + sbName.ToString() + ")";
                        strInfo.Add("L=" + segRectResults[i].rect_left + ",T=" + segRectResults[i].rect_top + ",W=" + segRectResults[i].rect_width + ",H=" + segRectResults[i].rect_height + "|(" + sbName.ToString().Replace("|", " ") + ")");
                    }
                    */
                    //pbxSrc.Refresh();
                    //MessageBox.Show(strInfo, "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    //MessageBox.Show("Segment Infer Failed", "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    DPInfer.Datum_Destroy(pDatumOut);
            }

            // Release datum
            DPInfer.Datum_Destroy(pDatumIn);
            return strInfo;
        }

        public List<string> GetDefectIdentResultForPerovskite(ref Bitmap bitmap, ref Bitmap bitmap2, ref List<Halcon_DefectIdentInfo> list)
        {
            List<string> strInfo = new List<string>();
            m_bitmapSrc = bitmap;
            Debug.Assert(m_model != IntPtr.Zero && m_bitmapSrc != null);

            m_liRect.Clear();
            //pbxSrc.Refresh();

            // Create datum
            DPShape sIn = DPInfer.Shape_CreateV2(1, 3, (uint)m_bitmapSrc.Height, (uint)m_bitmapSrc.Width);  // Channel = 3 for BGR24Bit image
            IntPtr pDatumIn = DPInfer.Datum_CreateV2(1);
            DPInfer.Datum_Reshape(pDatumIn, sIn);

            // Fill input image data
            BitmapData dataSrc = m_bitmapSrc.LockBits(new Rectangle(0, 0, m_bitmapSrc.Width, m_bitmapSrc.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            IntPtr pDataDst = DPInfer.Datum_GetData(pDatumIn);
            for (int i = 0; i < m_bitmapSrc.Height; i++)
                DPInfer.MemCopy((IntPtr)(pDataDst.ToInt64() + m_bitmapSrc.Width * 3 * i), (IntPtr)(dataSrc.Scan0.ToInt64() + dataSrc.Stride * i), (uint)m_bitmapSrc.Width * 3);
            m_bitmapSrc.UnlockBits(dataSrc);

            // Infer
            int iModelType = DPInfer.DnnInfer_GetModelType(m_model);
            if (iModelType == 0)        // Classify
            {
                int iLabelIndex = -1;
                int iClassNum = DPInfer.DnnInfer_GetClassNum(m_model);
                float[] fProbs = new float[iClassNum];
                if (true == DPInfer.DnnInfer_Cls_Infer(m_model, pDatumIn, ref iLabelIndex, fProbs))
                {
                    //strInfo = "Classify Infer Success";
                    if (iLabelIndex != -1)
                    {
                        StringBuilder sbName = new StringBuilder(16);
                        DPInfer.DnnInfer_GetLabelName(m_model, sbName, iLabelIndex);
                        //strInfo += " (" + sbName.ToString() + ":" + fProbs[iLabelIndex].ToString("F2") + ")";
                        strInfo.Add(" (" + sbName.ToString() + ":" + fProbs[iLabelIndex].ToString("F2") + ")");
                    }
                    //MessageBox.Show(strInfo, "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                }
                //MessageBox.Show("Classify Infer Failed", "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else if (iModelType == 1)   // Object detect
            {
                int iUseSize = 0;
                DPDetResult[] detResults = new DPDetResult[128];
                if (true == DPInfer.DnnInfer_Det_Infer(m_model, pDatumIn, detResults, 128, ref iUseSize))
                {
                    //strInfo = "ObjectDetect Infer Success";
                    for (int i = 0; i < iUseSize; i++)
                    {
                        m_liRect.Add(new Rectangle(detResults[i].left, detResults[i].top, detResults[i].width, detResults[i].height));
                        //strInfo += "\n" + i + ",L=" + detResults[i].left + ",T=" + detResults[i].top + ",W=" + detResults[i].width + ",H=" + detResults[i].height + " (" + detResults[i].name + ":" + detResults[i].score.ToString("F2") + ")";

                        strInfo.Add("L=" + detResults[i].left + ",T=" + detResults[i].top + ",W=" + detResults[i].width + ",H=" + detResults[i].height + "|(" + detResults[i].name + ":" + detResults[i].score.ToString("F2") + ")");
                    }
                    //pbxSrc.Refresh();
                    //MessageBox.Show(strInfo, "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    //MessageBox.Show("ObjectDetect Infer Failed", "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else if (iModelType == 2)   // Segment
            {
                DPShape sOut = DPInfer.Shape_CreateV2(1, 1, (uint)m_bitmapSrc.Height, (uint)m_bitmapSrc.Width);  // Channel = 1 for Gray image
                IntPtr pDatumOut = DPInfer.Datum_CreateV2(1);
                DPInfer.Datum_Reshape(pDatumOut, sOut);

                if (true == DPInfer.DnnInfer_Seg_Infer(m_model, pDatumIn, pDatumOut))
                {
                    // Fill output image data
                    if (m_bitmapDst != null)
                    {
                        m_bitmapDst.Dispose();
                        m_bitmapDst = null;
                    }
                    m_bitmapDst = new Bitmap(m_bitmapSrc.Width, m_bitmapSrc.Height, PixelFormat.Format8bppIndexed);
                    BitmapData dataTemp = m_bitmapDst.LockBits(new Rectangle(0, 0, m_bitmapDst.Width, m_bitmapDst.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                    IntPtr pDataTemp = DPInfer.Datum_GetData(pDatumOut);
                    for (int i = 0; i < m_bitmapDst.Height; i++)
                        DPInfer.MemCopy((IntPtr)(dataTemp.Scan0.ToInt64() + dataTemp.Stride * i), (IntPtr)(pDataTemp.ToInt64() + m_bitmapDst.Width * i), (uint)m_bitmapDst.Width);
                    m_bitmapDst.UnlockBits(dataTemp);
                    //bitmap2 = (Bitmap)m_bitmapDst.Clone();
                    bitmap2 = new Bitmap(m_bitmapDst.Width, m_bitmapDst.Height, PixelFormat.Format24bppRgb);
                    Graphics g1 = Graphics.FromImage(bitmap2);
                    g1.DrawImage(m_bitmapDst, 0, 0);
                    #region 先将创建的Bitmap通过Halcon转换成单通道灰度图
                    HObject ho_Image, ho_Image1;
                    // Initialize local and output iconic variables 
                    HOperatorSet.GenEmptyObj(out ho_Image);
                    HOperatorSet.GenEmptyObj(out ho_Image1);
                    ho_Image.Dispose();
                    ho_Image1.Dispose();
                    halconProvider.Bitmap2HObjectBpp8(m_bitmapDst, out ho_Image);
                    halconProvider.HObject2Bpp8_(ho_Image, out m_bitmapDst);
                    //bitmap = m_bitmapDst;
                    ho_Image.Dispose();
                    #endregion
                    dataTemp = m_bitmapDst.LockBits(new Rectangle(0, 0, m_bitmapDst.Width, m_bitmapDst.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                    pDataTemp = DPInfer.Datum_GetData(pDatumOut);
                    for (int i = 0; i < m_bitmapDst.Height; i++)
                        DPInfer.MemCopy((IntPtr)(dataTemp.Scan0.ToInt64() + dataTemp.Stride * i), (IntPtr)(pDataTemp.ToInt64() + m_bitmapDst.Width * i), (uint)m_bitmapDst.Width);
                    m_bitmapDst.UnlockBits(dataTemp);
                    bitmap = (Bitmap)m_bitmapDst.Clone();
                    // Parse object information
                    int iUseSize = 0;
                    float fontSize = 0;
                    if (!float.TryParse(ConfigurationManager.AppSettings["DIN_FONTSIZE"], out fontSize))
                        fontSize = 12;
                    DPSegRectResult[] segRectResults = new DPSegRectResult[10240];
                    DPInfer.DnnInfer_Seg_Label2Rect(m_model, pDatumOut, segRectResults, 10240, ref iUseSize);
                    //strInfo = "Segment Infer Success";
                    for (int i = 0; i < iUseSize; i++)
                    {
                        m_liRect.Add(new Rectangle(segRectResults[i].rect_left, segRectResults[i].rect_top, segRectResults[i].rect_width, segRectResults[i].rect_height));
                        StringBuilder sbName = new StringBuilder(16);
                        DPInfer.DnnInfer_GetLabelName(m_model, sbName, segRectResults[i].label);
                        string signTypeName = sbName.ToString().Replace("|", " ");
                        //strInfo += "\n" + i + ",L=" + segRectResults[i].rect_left + ",T=" + segRectResults[i].rect_top + ",W=" + segRectResults[i].rect_width + ",H=" + segRectResults[i].rect_height + " (" + sbName.ToString() + ")";
                        strInfo.Add("L=" + segRectResults[i].rect_left + ",T=" + segRectResults[i].rect_top + ",W=" + segRectResults[i].rect_width + ",H=" + segRectResults[i].rect_height + "|(" + signTypeName + ")");
                        #region 开始绘图，整体耗时小于1毫秒。
                        Color color = Color.Red;
                        string ColorValue = ConfigurationManager.AppSettings["DIN_"+signTypeName]+"";
                        if (ColorValue.Split('.').Length == 3)
                            color = Color.FromArgb(int.Parse(ColorValue.Split('.')[0]), int.Parse(ColorValue.Split('.')[1]), int.Parse(ColorValue.Split('.')[2]));
                        Pen pen = new Pen(color);
                        Brush brush = new SolidBrush(color);
                        Rectangle rectangle = new Rectangle(segRectResults[i].rect_left, segRectResults[i].rect_top, segRectResults[i].rect_width, segRectResults[i].rect_height);
                        g1.DrawRectangle(pen, rectangle);
                        
                        g1.DrawString(signTypeName, new Font("微软雅黑", fontSize), brush, segRectResults[i].rect_left + segRectResults[i].rect_width, segRectResults[i].rect_top + segRectResults[i].rect_height-1);
                        #endregion
                    }
                    g1.Dispose();
                    //pbxSrc.Refresh();
                    //MessageBox.Show(strInfo, "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    //MessageBox.Show("Segment Infer Failed", "DPInferCS", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    DPInfer.Datum_Destroy(pDatumOut);
            }

            // Release datum
            DPInfer.Datum_Destroy(pDatumIn);
            return strInfo;
        }

        public List<Halcon_DefectIdentInfo> GetDefectIdentByHalcon(Bitmap bitmap)
        {
            HObject ho_Image, ho_ImageRGB, ho_Region = null;
            HObject ho_ConnectedRegions = null, ho_ImageResult = null;

            // Local control variables 

            HTuple hv_Min = new HTuple(), hv_Max = new HTuple();
            HTuple hv_Range = new HTuple(), hv_Pointer = new HTuple();
            HTuple hv_Type = new HTuple(), hv_Width1 = new HTuple();
            HTuple hv_Height1 = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Number = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageRGB);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_ImageResult);
            ho_Image.Dispose();
            //HOperatorSet.ReadImage(out ho_Image, "C:/Users/12626/Desktop/d8da2566db01e8f4405e218ddbb56ac.png");
            halconProvider.Bitmap2HObjectBpp8(bitmap, out ho_Image);
            hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
            HOperatorSet.MinMaxGray(ho_Image, ho_Image, 0, out hv_Min, out hv_Max, out hv_Range);
            hv_Pointer.Dispose(); hv_Type.Dispose(); hv_Width1.Dispose(); hv_Height1.Dispose();
            HOperatorSet.GetImagePointer1(ho_Image, out hv_Pointer, out hv_Type, out hv_Width1,
                out hv_Height1);
            ho_ImageRGB.Dispose();
            HOperatorSet.GenImage3(out ho_ImageRGB, "byte", hv_Width1, hv_Height1, hv_Pointer,
                hv_Pointer, hv_Pointer);
            List<Halcon_DefectIdentInfo> list = new List<Halcon_DefectIdentInfo>();
            if ((int)(new HTuple(hv_Min.TupleLess(hv_Max))) != 0)
            {
                HTuple end_val7 = hv_Max;
                HTuple step_val7 = 1;
                for (hv_Index = 1; hv_Index.Continue(end_val7, step_val7); hv_Index = hv_Index.TupleAdd(step_val7))
                {

                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_Image, out ho_Region, hv_Index, hv_Index);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                    hv_Number.Dispose();
                    HOperatorSet.CountObj(ho_ConnectedRegions, out hv_Number);
                    Halcon_DefectIdentInfo halcon_DefectIdent = new Halcon_DefectIdentInfo() { DefectType = (ushort)hv_Index.D, defectTotal = (ushort)hv_Number.D };
                    list.Add(halcon_DefectIdent);
                    hv_Area.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                    HOperatorSet.AreaCenter(ho_ConnectedRegions, out hv_Area, out hv_Row, out hv_Column);
                    ho_ImageResult.Dispose();
                    HOperatorSet.PaintRegion(ho_ConnectedRegions, ho_ImageRGB, out ho_ImageResult,
                        ((new HTuple(255)).TupleConcat(255)).TupleConcat(0), "margin");
                }
            }
            else
            {
                //图片判断为OK
            }
            ho_Image.Dispose();
            ho_ImageRGB.Dispose();
            ho_Region.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_ImageResult.Dispose();

            hv_Min.Dispose();
            hv_Max.Dispose();
            hv_Range.Dispose();
            hv_Pointer.Dispose();
            hv_Type.Dispose();
            hv_Width1.Dispose();
            hv_Height1.Dispose();
            hv_Index.Dispose();
            hv_Number.Dispose();
            hv_Area.Dispose();
            hv_Row.Dispose();
            hv_Column.Dispose();
            return list;
        }

        public Halcon_DefectIdentInfo GetDefectIdentByHalcon(Bitmap bitmapTSD, Bitmap bitmapTotal, ref Bitmap bitmap)
        {

            HObject ho_TSD, ho_Totle, ho_TSDRegion, ho_TDRegion;
            HObject ho_BPDRegion, ho_TEDRegion, ho_ConnectedRegions;
            HObject ho_ConnectedRegions1, ho_ConnectedRegions2, ho_TotleRegion;
            HObject ho_ImageCleared, ho_TED, ho_BPD, ho_MultiChannelImage;

            // Local control variables 

            HTuple hv_Min = new HTuple(), hv_Max = new HTuple();
            HTuple hv_Range = new HTuple(), hv_Min1 = new HTuple();
            HTuple hv_Max1 = new HTuple(), hv_Range1 = new HTuple();
            HTuple hv_TSDNumber = new HTuple(), hv_TEDNumber = new HTuple();
            HTuple hv_BPDNumber = new HTuple(), hv_TSDArea = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_TEDArea = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_BPDArea = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_TSD);
            HOperatorSet.GenEmptyObj(out ho_Totle);
            HOperatorSet.GenEmptyObj(out ho_TSDRegion);
            HOperatorSet.GenEmptyObj(out ho_TDRegion);
            HOperatorSet.GenEmptyObj(out ho_BPDRegion);
            HOperatorSet.GenEmptyObj(out ho_TEDRegion);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_TotleRegion);
            HOperatorSet.GenEmptyObj(out ho_ImageCleared);
            HOperatorSet.GenEmptyObj(out ho_TED);
            HOperatorSet.GenEmptyObj(out ho_BPD);
            HOperatorSet.GenEmptyObj(out ho_MultiChannelImage);
            ho_TSD.Dispose();
            //HOperatorSet.ReadImage(out ho_TSD, "C:/Users/12626/Desktop/1.png");
            halconProvider.Bitmap2HObjectBpp8(bitmapTSD, out ho_TSD);
            ho_Totle.Dispose();
            //HOperatorSet.ReadImage(out ho_Totle, "C:/Users/12626/Desktop/3.png");
            halconProvider.Bitmap2HObjectBpp8(bitmapTotal, out ho_Totle);
            hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
            HOperatorSet.MinMaxGray(ho_TSD, ho_TSD, 0, out hv_Min, out hv_Max, out hv_Range);
            hv_Min1.Dispose(); hv_Max1.Dispose(); hv_Range1.Dispose();
            HOperatorSet.MinMaxGray(ho_Totle, ho_Totle, 0, out hv_Min1, out hv_Max1, out hv_Range1);
            ho_TSDRegion.Dispose();
            HOperatorSet.Threshold(ho_TSD, out ho_TSDRegion, 1, 1);
            ho_TDRegion.Dispose();
            HOperatorSet.Threshold(ho_Totle, out ho_TDRegion, 1, 1);
            ho_BPDRegion.Dispose();
            HOperatorSet.Threshold(ho_Totle, out ho_BPDRegion, 2, 2);
            {
                HObject ExpTmpOutVar_0;
                HOperatorSet.DilationCircle(ho_TSDRegion, out ExpTmpOutVar_0, 10);
                ho_TSDRegion.Dispose();
                ho_TSDRegion = ExpTmpOutVar_0;
            }
            ho_TEDRegion.Dispose();
            HOperatorSet.Difference(ho_TDRegion, ho_TSDRegion, out ho_TEDRegion);

            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_TSDRegion, out ho_ConnectedRegions);
            ho_ConnectedRegions1.Dispose();
            HOperatorSet.Connection(ho_TEDRegion, out ho_ConnectedRegions1);
            ho_ConnectedRegions2.Dispose();
            HOperatorSet.Connection(ho_BPDRegion, out ho_ConnectedRegions2);

            hv_TSDNumber.Dispose();
            HOperatorSet.CountObj(ho_ConnectedRegions, out hv_TSDNumber);
            hv_TEDNumber.Dispose();
            HOperatorSet.CountObj(ho_ConnectedRegions1, out hv_TEDNumber);
            hv_BPDNumber.Dispose();
            HOperatorSet.CountObj(ho_ConnectedRegions2, out hv_BPDNumber);

            hv_TSDArea.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
            HOperatorSet.AreaCenter(ho_TSDRegion, out hv_TSDArea, out hv_Row, out hv_Column);
            hv_TEDArea.Dispose(); hv_Row1.Dispose(); hv_Column1.Dispose();
            HOperatorSet.AreaCenter(ho_TEDRegion, out hv_TEDArea, out hv_Row1, out hv_Column1);
            hv_BPDArea.Dispose(); hv_Row2.Dispose(); hv_Column2.Dispose();
            HOperatorSet.AreaCenter(ho_BPDRegion, out hv_BPDArea, out hv_Row2, out hv_Column2);
            ho_TotleRegion.Dispose();
            HOperatorSet.Union2(ho_TDRegion, ho_TSDRegion, out ho_TotleRegion);

            hv_Width.Dispose(); hv_Height.Dispose();
            HOperatorSet.GetImageSize(ho_Totle, out hv_Width, out hv_Height);
            ho_ImageCleared.Dispose();
            HOperatorSet.GenImageProto(ho_Totle, out ho_ImageCleared, 0);
            ho_TSD.Dispose();
            HOperatorSet.PaintRegion(ho_TSDRegion, ho_ImageCleared, out ho_TSD, 255, "fill");//红
            ho_TED.Dispose();
            HOperatorSet.PaintRegion(ho_TEDRegion, ho_ImageCleared, out ho_TED, 255, "fill");//蓝
            ho_BPD.Dispose();
            HOperatorSet.PaintRegion(ho_BPDRegion, ho_ImageCleared, out ho_BPD, 255, "fill");//绿
            ho_MultiChannelImage.Dispose();
            HOperatorSet.Compose3(ho_TSD, ho_BPD, ho_TED, out ho_MultiChannelImage);
            halconProvider.HObject2Bpp8_(ho_MultiChannelImage, out bitmap);
            ho_TSD.Dispose();
            ho_Totle.Dispose();
            ho_TSDRegion.Dispose();
            ho_TDRegion.Dispose();
            ho_BPDRegion.Dispose();
            ho_TEDRegion.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_ConnectedRegions1.Dispose();
            ho_ConnectedRegions2.Dispose();
            ho_TotleRegion.Dispose();
            ho_ImageCleared.Dispose();
            ho_TED.Dispose();
            ho_BPD.Dispose();
            ho_MultiChannelImage.Dispose();

            hv_Min.Dispose();
            hv_Max.Dispose();
            hv_Range.Dispose();
            hv_Min1.Dispose();
            hv_Max1.Dispose();
            hv_Range1.Dispose();
            hv_TSDNumber.Dispose();
            hv_TEDNumber.Dispose();
            hv_BPDNumber.Dispose();
            hv_TSDArea.Dispose();
            hv_Row.Dispose();
            hv_Column.Dispose();
            hv_TEDArea.Dispose();
            hv_Row1.Dispose();
            hv_Column1.Dispose();
            hv_BPDArea.Dispose();
            hv_Row2.Dispose();
            hv_Column2.Dispose();
            hv_Width.Dispose();
            hv_Height.Dispose();
            return null;
        }
    }

    public class Halcon_DefectIdentInfo
    {
        public string serialNum { get; set; }
        public int areaId { get; set; }
        public ushort DefectType { get; set; }
        public List<long> AreaInfoList { get; set; }

        public List<double> PointXList { get; set; }
        public List<double> PointYList { get; set; }

        public long defectTotal { get; set; }

        public void Save()
        {
            DefectInfo defectIdentInfo = new DefectInfo()
            {
                areaId = areaId,
                serialNum = serialNum,
                //defectInfo = String.Join(",", AreaInfoList) + "|" + String.Join(",", PointXList) + "|" + String.Join(",", PointYList),
                defectTotal = defectTotal,
                createTime = DateTime.Now,
                defectType = DefectType,
            };
            defectIdentInfo.SaveModelM();
        }
    }

    public class DefectInfo
    {
        public int id { get; set; }
        public int areaId { get; set; }
        public string serialNum { get; set; }
        public int defectType { get; set; }
        public string defectInfo { get; set; }
        public long defectTotal { get; set; }


        public DateTime createTime { get; set; }
    }
}
