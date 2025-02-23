//
// File generated by HDevelop for HALCON/.NET (C#) Version 22.11.1.0
// Non-ASCII strings in this file are encoded in local-8-bit encoding (cp936).
// 
// Please note that non-ASCII characters in string constants are exported
// as octal codes in order to guarantee that the strings are correctly
// created on all systems, independent on any compiler settings.
// 
// Source files with different encoding should not be mixed in one project.
//

using HalconDotNet;
using System.Drawing;
using TimeTech.Core.Common;

public partial class HDevelopExport
{

    HalconProvider halconProvider = new TimeTech.Core.Common.HalconProvider();

    // Main procedure 
    public void actionShow(Bitmap btDefOrg,string hobj,Bitmap bitmap2,ref Bitmap bitDefImage)
  {


    // Local iconic variables 

    HObject ho_ImageY, ho_Obj, ho_ImageQ, ho_ImageReduced;
    HObject ho_ImageResult, ho_Image;

    // Local control variables 

    HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
    HTuple hv_WindowHandle = new HTuple();
    // Initialize local and output iconic variables 
    HOperatorSet.GenEmptyObj(out ho_ImageY);
    HOperatorSet.GenEmptyObj(out ho_Obj);
    HOperatorSet.GenEmptyObj(out ho_ImageQ);
    HOperatorSet.GenEmptyObj(out ho_ImageReduced);
    HOperatorSet.GenEmptyObj(out ho_ImageResult);
    HOperatorSet.GenEmptyObj(out ho_Image);
    ho_ImageY.Dispose();
        //HOperatorSet.ReadImage(out ho_ImageY, "D:/项目/钙钛矿/发布图-PS提亮/BL/0.png");
        halconProvider.Bitmap2HObjectBpp24(btDefOrg, out ho_ImageY);
        //原图:ImageY
        ho_Obj.Dispose();
    //HOperatorSet.ReadObject(out ho_Obj, "D:/项目/钙钛矿/发布图-PS提亮/BL/0.hobj");
        HOperatorSet.ReadObject(out ho_Obj, hobj);
        //保存的位置信息文件Obj
        ho_ImageQ.Dispose();
        halconProvider.Bitmap2HObjectBpp24(bitmap2, out ho_ImageQ);
        //HOperatorSet.ReadImage(out ho_ImageQ, "D:/项目/钙钛矿/发布图-PS提亮/BL/易渊彩图/0.png");
    //易渊彩图ImageQ

    hv_Width.Dispose();hv_Height.Dispose();
    HOperatorSet.GetImageSize(ho_ImageY, out hv_Width, out hv_Height);
    ho_ImageReduced.Dispose();
    HOperatorSet.ReduceDomain(ho_ImageQ, ho_Obj, out ho_ImageReduced);
    ho_ImageResult.Dispose();
    HOperatorSet.AddImage(ho_ImageY, ho_ImageReduced, out ho_ImageResult, 1, 0);
    hv_WindowHandle.Dispose();
    HOperatorSet.OpenWindow(0, 0, hv_Width, hv_Height, 0, "buffer", "", out hv_WindowHandle);
    HOperatorSet.SetColor(hv_WindowHandle, "white");
    HOperatorSet.SetDraw(hv_WindowHandle, "fill");
    HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_Width, hv_Height);
    HOperatorSet.DispObj(ho_ImageY, hv_WindowHandle);
    HOperatorSet.DispObj(ho_ImageResult, hv_WindowHandle);
    ho_Image.Dispose();
    HOperatorSet.DumpWindowImage(out ho_Image, hv_WindowHandle);
        //结果图Image
        halconProvider.HObject2Bpp24(ho_Image, out bitDefImage);
    ho_ImageY.Dispose();
    ho_Obj.Dispose();
    ho_ImageQ.Dispose();
    ho_ImageReduced.Dispose();
    ho_ImageResult.Dispose();
    ho_Image.Dispose();

    hv_Width.Dispose();
    hv_Height.Dispose();
    hv_WindowHandle.Dispose();

  }




}


