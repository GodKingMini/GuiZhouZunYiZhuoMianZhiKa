using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sporcard
{
    public class Info
    {
        public string name = "";                          //姓名
        public string idcard = "";                        //身份证
        public string isOK = "";                          //是否成功
        public string sbyy = "";                          //失败原因
        public string sbkh = "";                          //社保卡号
        public string ksbm = "";                          //卡识别码
        public string atr = "";                           //ATR
        public string bankcardid = "";                    //银行卡号
        public string folk = "";                           //民族
        public string brithday = "";                       //出生日期
        public string education = "";                      //学历
        public string hknature = "";                       //户口性质
        public string sex = "";                            //性别
        public string nationality = "";                    //国籍
        public string hkaddress = "";                      //户口所在地地址
        public string postcode = "";                       //常住所在地邮政编码
        public string status = "";                         //个人就业或离退休（职）状态
        public string phone = "";                          //联系电话
        public string address = "";                        //联系地址
        public string administrativecode = "";             //行政区划代码
        public string unidcode = "";                       //单位编码
        public string unitname = "";                       //单位名称
        public string bankcode = "";                       //银行标识码
        public string issuercode = "";                     //发行机构标识号
        public string cardcategory = "";                   //卡的类别
        public string cardversion = "";                    //卡片版本
        public string kyxq = "";                           //卡有效期
        public string photo = "";                          //照片

        public string wdcode = "";                         //网点编码
        public string printname = "";                      //制卡机名称
        public string printcode = "";                      //制卡机编码


        public void Clear()
        {
            name = "";                          //姓名
            idcard = "";                        //身份证
            isOK = "";                          //是否成功
            sbyy = "";                          //失败原因
            sbkh = "";                          //社保卡号
            ksbm = "";                          //卡识别码
            atr = "";                           //ATR
            bankcardid = "";                    //银行卡号
            folk = "";                           //民族
            brithday = "";                       //出生日期
            education = "";                      //学历
            hknature = "";                       //户口性质
            sex = "";                            //性别
            nationality = "";                    //国籍
            hkaddress = "";                      //户口所在地地址
            postcode = "";                       //常住所在地邮政编
            status = "";                         //个人就业或离退休
            phone = "";                          //联系电话
            address = "";                        //联系地址
            administrativecode = "";             //行政区划代码
            unidcode = "";                       //单位编码
            unitname = "";                       //单位名称
            bankcode = "";                       //银行标识码
            issuercode = "";                     //发行机构标识号
            cardcategory = "";                   //卡的类别
            cardversion = "";                    //卡片版本
            kyxq = "";                           //卡有效期
            photo = "";                          //照片
        }
    }
}