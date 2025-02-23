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
using System;
using System.Drawing;

namespace PerovskiteTest
{
    public partial class CheckCameraFocus
    {
        public CameraIsFocusOBJ CameraIsFocus(HObject ho_Image, ushort FocusValue)
        {
            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Image1, ho_Image2, ho_Image3;
            HObject ho_ImagePart00, ho_ImagePart01, ho_ImagePart10;
            HObject ho_ImageSub1, ho_ImageResult1, ho_ImageSub2, ho_ImageResult2;
            HObject ho_ImageResult;

            // Local control variables 

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_Value = new HTuple(), hv_Deviation = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_Image2);
            HOperatorSet.GenEmptyObj(out ho_Image3);
            HOperatorSet.GenEmptyObj(out ho_ImagePart00);
            HOperatorSet.GenEmptyObj(out ho_ImagePart01);
            HOperatorSet.GenEmptyObj(out ho_ImagePart10);
            HOperatorSet.GenEmptyObj(out ho_ImageSub1);
            HOperatorSet.GenEmptyObj(out ho_ImageResult1);
            HOperatorSet.GenEmptyObj(out ho_ImageSub2);
            HOperatorSet.GenEmptyObj(out ho_ImageResult2);
            HOperatorSet.GenEmptyObj(out ho_ImageResult);
            //ho_Image.Dispose();
            //HOperatorSet.ReadImage(out ho_Image, "D:\\work\\program\\PerovskiteTest\\bin\\Debug\\IMG\\20231221 14\\明场\\ORGIMG\\0 .png".Replace("\\","/"));
            ho_Image1.Dispose(); ho_Image2.Dispose(); ho_Image3.Dispose();
            HOperatorSet.Decompose3(ho_Image, out ho_Image1, out ho_Image2, out ho_Image3
                );
            hv_Width.Dispose(); hv_Height.Dispose();
            HOperatorSet.GetImageSize(ho_Image2, out hv_Width, out hv_Height);
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_ImagePart00.Dispose();
                HOperatorSet.CropPart(ho_Image2, out ho_ImagePart00, 0, 0, hv_Width - 1, hv_Height - 1);
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_ImagePart01.Dispose();
                HOperatorSet.CropPart(ho_Image2, out ho_ImagePart01, 0, 1, hv_Width - 1, hv_Height - 1);
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_ImagePart10.Dispose();
                HOperatorSet.CropPart(ho_Image2, out ho_ImagePart10, 1, 0, hv_Width - 1, hv_Height - 1);
            }
            {
                HObject ExpTmpOutVar_0;
                HOperatorSet.ConvertImageType(ho_ImagePart00, out ExpTmpOutVar_0, "real");
                ho_ImagePart00.Dispose();
                ho_ImagePart00 = ExpTmpOutVar_0;
            }
            {
                HObject ExpTmpOutVar_0;
                HOperatorSet.ConvertImageType(ho_ImagePart10, out ExpTmpOutVar_0, "real");
                ho_ImagePart10.Dispose();
                ho_ImagePart10 = ExpTmpOutVar_0;
            }
            {
                HObject ExpTmpOutVar_0;
                HOperatorSet.ConvertImageType(ho_ImagePart01, out ExpTmpOutVar_0, "real");
                ho_ImagePart01.Dispose();
                ho_ImagePart01 = ExpTmpOutVar_0;
            }
            ho_ImageSub1.Dispose();
            HOperatorSet.SubImage(ho_ImagePart10, ho_ImagePart00, out ho_ImageSub1, 1, 0);
            ho_ImageResult1.Dispose();
            HOperatorSet.MultImage(ho_ImageSub1, ho_ImageSub1, out ho_ImageResult1, 1, 0);
            ho_ImageSub2.Dispose();
            HOperatorSet.SubImage(ho_ImagePart01, ho_ImagePart00, out ho_ImageSub2, 1, 0);
            ho_ImageResult2.Dispose();
            HOperatorSet.MultImage(ho_ImageSub2, ho_ImageSub2, out ho_ImageResult2, 1, 0);
            ho_ImageResult.Dispose();
            HOperatorSet.AddImage(ho_ImageResult1, ho_ImageResult2, out ho_ImageResult, 1,
                0);
            hv_Value.Dispose(); hv_Deviation.Dispose();
            HOperatorSet.Intensity(ho_ImageResult, ho_ImageResult, out hv_Value, out hv_Deviation);

            CameraIsFocusOBJ objReturn = new CameraIsFocusOBJ() { Eval = hv_Value.D, FocusValue = FocusValue };
            ho_Image.Dispose();
            ho_Image1.Dispose();
            ho_Image2.Dispose();
            ho_Image3.Dispose();
            ho_ImagePart00.Dispose();
            ho_ImagePart01.Dispose();
            ho_ImagePart10.Dispose();
            ho_ImageSub1.Dispose();
            ho_ImageResult1.Dispose();
            ho_ImageSub2.Dispose();
            ho_ImageResult2.Dispose();
            ho_ImageResult.Dispose();

            hv_Width.Dispose();
            hv_Height.Dispose();
            hv_Value.Dispose();
            hv_Deviation.Dispose();


            return objReturn;
        }
    }

        public class CameraIsFocusOBJ
        {
            /// <summary>
            /// 焦距
            /// </summary>
            public ushort FocusValue { get; set; }
            /// <summary>
            /// 识别图地址
            /// </summary>
            public string ImgPath { get; set; }
            /// <summary>
            /// 评估值
            /// </summary>
            public double Eval { get; set; }

        }

    }
