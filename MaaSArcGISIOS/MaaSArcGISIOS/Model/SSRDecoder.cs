using System;
using System.Collections.Generic;
using System.Text;

namespace MaaSArcGISIOS.Model
{
    public class SignalBiasData
    {
        public int gs { set; get; }
        public int prn { set; get; }
        public int signalIndex { set; get; }
        public double codeBias { set; get; }
        public double phaseBias { set; get; }

        public override string ToString()
        {
            return string.Format("gs:{0},prn:{1},signalIndex:{2},codeBias:{3},phaseBias:{4}", this.gs, this.prn, this.signalIndex, this.codeBias, this.phaseBias);
        }
    }

    public class OrbitCorrectionData
    {
        public int gs { set; get; }
        public int prn { set; get; }
        public int iod { set; get; }
        public double radial { set; get; }
        public double alongTrack { set; get; }
        public double crossTrack { set; get; }

        public override string ToString()
        {
            return string.Format("gs:{0},prn:{1},iod:{2},radial:{3},alongTrack:{4},crossTrack:{5}", this.gs, this.prn, this.iod, this.radial, this.alongTrack, this.crossTrack);
        }
    }

    public class IonosphereDelayData
    {
        /// <summary>
        /// GPS Week Second
        /// </summary>
        public int gs { set; get; }
        /// <summary>
        /// 격자점 위도
        /// </summary>
        public int latitude { set; get; }
        /// <summary>
        /// 격저잠 경도
        /// </summary>
        public int longitude { set; get; }
        public int height { set; get; }
        /// <summary>
        /// 위성번호
        /// </summary>
        public int prn { set; get; }
        /// <summary>
        /// 전리층 지연량
        /// </summary>
        public double stec { set; get; }

        public override string ToString()
        {
            return string.Format("gs:{0},prn:{1},latitude:{2},longitude:{3},height:{4},stec:{5}", this.gs, this.prn, this.latitude, this.longitude, this.height, this.stec);
        }
    }

    public class ClockCorrectionData
    {
        public int gs { set; get; }
        public int prn { set; get; }
        public double deltaClockC0 { set; get; }
        public double deltaClockC1 { set; get; }
        public double deltaClockC2 { set; get; }

        public override string ToString()
        {
            return string.Format("gs:{0},prn:{1},deltaClockC0:{2},deltaClockC1:{3},deltaClockC2:{4}", this.gs, this.prn, this.deltaClockC0, this.deltaClockC1, this.deltaClockC2);
        }
    }

    public class TroposphereDelayData
    {
        /// <summary>
        /// GPS Week Second
        /// </summary>
        public int gs { set; get; }
        /// <summary>
        /// 격자점 위도
        /// </summary>
        public int latitude { set; get; }
        /// <summary>
        /// 격저잠 경도
        /// </summary>
        public int longitude { set; get; }
        public int height { set; get; }
        /// <summary>
        /// 대류권 건조 지연량
        /// </summary>
        public double ztd { set; get; }
        /// <summary>
        /// 대류권 습윤 지연량
        /// </summary>
        public double zwd { set; get; }


        public override string ToString()
        {
            return string.Format("gs:{0},latitude:{1},longitude:{2},height:{3},ztd:{4},zwd:{5}", this.gs, this.latitude, this.longitude, this.height, this.ztd, this.zwd);
        }
    }

    public class SSRDecoder
    {
        public static void SSRBufferDecoder(byte[] pBuffer, Action<SSRDecoderObject> pAction)
        {
            int pos = 0;
            int datalength, start, end, crc, msgno;
            int totallength = pBuffer.Length;
            while (pos < totallength)
            {
                int preamble = 0;

                while (pos < totallength && preamble != 0xD3)
                {
                    preamble = BinaryConvertToInt(pBuffer, pos, 1);
                    pos++;
                }

                if (pos >= totallength - 1)
                {
                    break;
                }

                datalength = BinaryConvertToInt(pBuffer, pos, 2) & 0x03ff;
                if (pos + 2 + datalength + 3 > totallength)
                {
                    break;
                }

                pos = pos + 2;
                start = pos - 3;
                end = start + datalength + 3;
                byte[] tempbuffer = new byte[datalength];
                Buffer.BlockCopy(pBuffer, pos, tempbuffer, 0, datalength);
                crc = BinaryConvertToInt(pBuffer, end, 3);
                msgno = GetMessageNumber(tempbuffer);
                bool validcheck = CheckCRC24(pBuffer, start, end, crc);
                if (validcheck)
                {
                    pAction(new SSRDecoderObject(tempbuffer, crc, msgno));
                }
                else
                {
                    break;
                }
                pos = pos + datalength + 3;
            }
        }

        private static int BinaryConvertToInt(byte[] pBuffer, int pIndex, int pLength)
        {
            int result = 0;
            switch (pLength)
            {
                case 2:
                    result |= (pBuffer[pIndex] & 0xff) << 8;
                    result |= pBuffer[pIndex + 1] & 0xff;
                    break;
                case 3:
                    result |= (pBuffer[pIndex] & 0xff) << 16;
                    result |= (pBuffer[pIndex + 1] & 0xff) << 8;
                    result |= pBuffer[pIndex + 2] & 0xff;
                    break;
                case 4:
                    result |= (pBuffer[pIndex] & 0xff) << 24;
                    result |= (pBuffer[pIndex + 1] & 0xff) << 16;
                    result |= (pBuffer[pIndex + 2] & 0xff) << 8;
                    result |= pBuffer[pIndex + 3] & 0xff;
                    break;
                default:
                    result |= pBuffer[pIndex] & 0xff;
                    break;
            }
            return result;
        }

        private static int GetMessageNumber(byte[] pBuffer)
        {
            if (pBuffer == null)
                return 0;

            if (pBuffer.Length < 2)
                return 0;

            return BinaryConvertToInt(pBuffer, 0, 2) >> 4;
        }

        private static bool CheckCRC24(byte[] pBuffer, int pStart, int pEnd, int pCRC)
        {
            int[] TABLE = {
                0x00000000, 0x00864CFB, 0x008AD50D, 0x000C99F6, 0x0093E6E1, 0x0015AA1A,
                0x001933EC, 0x009F7F17, 0x00A18139, 0x0027CDC2, 0x002B5434, 0x00AD18CF,
                0x003267D8, 0x00B42B23, 0x00B8B2D5, 0x003EFE2E, 0x00C54E89, 0x00430272,
                0x004F9B84, 0x00C9D77F, 0x0056A868, 0x00D0E493, 0x00DC7D65, 0x005A319E,
                0x0064CFB0, 0x00E2834B, 0x00EE1ABD, 0x00685646, 0x00F72951, 0x007165AA,
                0x007DFC5C, 0x00FBB0A7, 0x000CD1E9, 0x008A9D12, 0x008604E4, 0x0000481F,
                0x009F3708, 0x00197BF3, 0x0015E205, 0x0093AEFE, 0x00AD50D0, 0x002B1C2B,
                0x002785DD, 0x00A1C926, 0x003EB631, 0x00B8FACA, 0x00B4633C, 0x00322FC7,
                0x00C99F60, 0x004FD39B, 0x00434A6D, 0x00C50696, 0x005A7981, 0x00DC357A,
                0x00D0AC8C, 0x0056E077, 0x00681E59, 0x00EE52A2, 0x00E2CB54, 0x006487AF,
                0x00FBF8B8, 0x007DB443, 0x00712DB5, 0x00F7614E, 0x0019A3D2, 0x009FEF29,
                0x009376DF, 0x00153A24, 0x008A4533, 0x000C09C8, 0x0000903E, 0x0086DCC5,
                0x00B822EB, 0x003E6E10, 0x0032F7E6, 0x00B4BB1D, 0x002BC40A, 0x00AD88F1,
                0x00A11107, 0x00275DFC, 0x00DCED5B, 0x005AA1A0, 0x00563856, 0x00D074AD,
                0x004F0BBA, 0x00C94741, 0x00C5DEB7, 0x0043924C, 0x007D6C62, 0x00FB2099,
                0x00F7B96F, 0x0071F594, 0x00EE8A83, 0x0068C678, 0x00645F8E, 0x00E21375,
                0x0015723B, 0x00933EC0, 0x009FA736, 0x0019EBCD, 0x008694DA, 0x0000D821,
                0x000C41D7, 0x008A0D2C, 0x00B4F302, 0x0032BFF9, 0x003E260F, 0x00B86AF4,
                0x002715E3, 0x00A15918, 0x00ADC0EE, 0x002B8C15, 0x00D03CB2, 0x00567049,
                0x005AE9BF, 0x00DCA544, 0x0043DA53, 0x00C596A8, 0x00C90F5E, 0x004F43A5,
                0x0071BD8B, 0x00F7F170, 0x00FB6886, 0x007D247D, 0x00E25B6A, 0x00641791,
                0x00688E67, 0x00EEC29C, 0x003347A4, 0x00B50B5F, 0x00B992A9, 0x003FDE52,
                0x00A0A145, 0x0026EDBE, 0x002A7448, 0x00AC38B3, 0x0092C69D, 0x00148A66,
                0x00181390, 0x009E5F6B, 0x0001207C, 0x00876C87, 0x008BF571, 0x000DB98A,
                0x00F6092D, 0x007045D6, 0x007CDC20, 0x00FA90DB, 0x0065EFCC, 0x00E3A337,
                0x00EF3AC1, 0x0069763A, 0x00578814, 0x00D1C4EF, 0x00DD5D19, 0x005B11E2,
                0x00C46EF5, 0x0042220E, 0x004EBBF8, 0x00C8F703, 0x003F964D, 0x00B9DAB6,
                0x00B54340, 0x00330FBB, 0x00AC70AC, 0x002A3C57, 0x0026A5A1, 0x00A0E95A,
                0x009E1774, 0x00185B8F, 0x0014C279, 0x00928E82, 0x000DF195, 0x008BBD6E,
                0x00872498, 0x00016863, 0x00FAD8C4, 0x007C943F, 0x00700DC9, 0x00F64132,
                0x00693E25, 0x00EF72DE, 0x00E3EB28, 0x0065A7D3, 0x005B59FD, 0x00DD1506,
                0x00D18CF0, 0x0057C00B, 0x00C8BF1C, 0x004EF3E7, 0x00426A11, 0x00C426EA,
                0x002AE476, 0x00ACA88D, 0x00A0317B, 0x00267D80, 0x00B90297, 0x003F4E6C,
                0x0033D79A, 0x00B59B61, 0x008B654F, 0x000D29B4, 0x0001B042, 0x0087FCB9,
                0x001883AE, 0x009ECF55, 0x009256A3, 0x00141A58, 0x00EFAAFF, 0x0069E604,
                0x00657FF2, 0x00E33309, 0x007C4C1E, 0x00FA00E5, 0x00F69913, 0x0070D5E8,
                0x004E2BC6, 0x00C8673D, 0x00C4FECB, 0x0042B230, 0x00DDCD27, 0x005B81DC,
                0x0057182A, 0x00D154D1, 0x0026359F, 0x00A07964, 0x00ACE092, 0x002AAC69,
                0x00B5D37E, 0x00339F85, 0x003F0673, 0x00B94A88, 0x0087B4A6, 0x0001F85D,
                0x000D61AB, 0x008B2D50, 0x00145247, 0x00921EBC, 0x009E874A, 0x0018CBB1,
                0x00E37B16, 0x006537ED, 0x0069AE1B, 0x00EFE2E0, 0x00709DF7, 0x00F6D10C,
                0x00FA48FA, 0x007C0401, 0x0042FA2F, 0x00C4B6D4, 0x00C82F22, 0x004E63D9,
                0x00D11CCE, 0x00575035, 0x005BC9C3, 0x00DD8538
            };

            int result = 0;
            for (int i = pStart; i < pEnd; i++)
            {
                result = TABLE[((result >> 16) ^ pBuffer[i]) & 0xFF] ^ (result << 8);
            }

            byte[] crcResult = toByteArray(result);
            byte[] crc24 = toByteArray(pCRC);

            if (crcResult.Length == crc24.Length)
            {
                bool check = true;
                for (int i = 0; i < crc24.Length; i++)
                {
                    if (crcResult[i] != crc24[i])
                    {
                        check = false;
                        break;
                    }
                }

                if (check)
                {
                    return true;
                }
            }
            return false;
        }
        public static byte[] toByteArray(int result)
        {
            return new byte[] { (byte)((result >> 16) & 0xFF), (byte)((result >> 8) & 0xFF), (byte)(result & 0xFF) };
        }
    }


    public class SSRDecoderObject
    {
        public string mBuffer { get; set; }
        public int mCRC24 { get; set; }
        public int mMsgNumber { get; set; }

        public SSRDecoderObject(byte[] pBuffer, int pCRC24, int pMsgNumber)
        {
            this.mBuffer = byteArrayToBinaryString(pBuffer);
            this.mCRC24 = pCRC24;
            this.mMsgNumber = pMsgNumber;
        }

        private string byteArrayToBinaryString(byte[] b)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte item in b)
            {
                sb.Append(byteToBinaryString(item));
            }
            return sb.ToString();
        }

        private string byteToBinaryString(byte n)
        {
            StringBuilder sb = new StringBuilder("00000000");
            for (int bit = 0; bit < 8; bit++)
            {
                if (((n >> bit) & 1) > 0)
                {
                    sb[7 - bit] = '1';
                }
            }
            return sb.ToString();
        }
    }

    public class SSRParser
    {
        public static void GetSSRObjectParserData(SSRDecoderObject pSSRDecoderObject
                                                , Action<Model.ClockCorrectionData> pClockCallback
                                                , Action<Model.IonosphereDelayData> pIonospherCallback
                                                , Action<Model.OrbitCorrectionData> pOrbitCallback
                                                , Action<Model.SignalBiasData> pSignalCallback
                                                , Action<Model.TroposphereDelayData> pTroposphereCallback)
        {
            int mNo = 0;
            if (pSSRDecoderObject.mMsgNumber == 4090)
            {
                mNo = readUBits(pSSRDecoderObject.mBuffer, 16, 8);
            }

            Console.WriteLine("mNo : {0}", mNo);

            switch (mNo)
            {
                case 1:
                    ParseSM001(pSSRDecoderObject.mBuffer, pOrbitCallback);
                    break;
                case 2:
                    ParseSM002(pSSRDecoderObject.mBuffer, pClockCallback);
                    break;
                case 3:
                    parseSM003(pSSRDecoderObject.mBuffer, pSignalCallback);
                    break;
                case 4:
                    parseSM004(pSSRDecoderObject.mBuffer, pTroposphereCallback);
                    break;
                case 5:
                    parseSM005(pSSRDecoderObject.mBuffer, pIonospherCallback);
                    break;
                case 6:
                    parseSM006(pSSRDecoderObject.mBuffer, pTroposphereCallback, pIonospherCallback);
                    break;
                default:
                    break;
            }
        }
        // sato
        private static void ParseSM001(string pBuffer, Action<Model.OrbitCorrectionData> pOrbitCallback)
        {
            int ver = readUBits(pBuffer, 24, 3);
            int gs = readUBits(pBuffer, 27, 20);
            int gnssIndicator = readUBits(pBuffer, 57, 4);
            int noSat = readUBits(pBuffer, 62, 6);

            int pos = 68;
            int size = (ver == 0) ? 8 : 11;
            for (int i = 0; i < noSat; i++)
            {
                int prn = _prnHeader(gnssIndicator) + readUBits(pBuffer, pos, 6);
                int iod = readUBits(pBuffer, pos + 6, size);
                double rad = readBits(pBuffer, pos + 6 + size, 22) * 0.0001;
                double alt = readBits(pBuffer, pos + 28 + size, 20) * 0.0004;
                double crt = readBits(pBuffer, pos + 48 + size, 20) * 0.0004;

                pOrbitCallback(new OrbitCorrectionData()
                {
                    gs = gs,
                    prn = prn,
                    iod = iod,
                    radial = rad,
                    alongTrack = alt,
                    crossTrack = crt,
                });

                pos += 68 + size;
            }
        }
        // satc
        private static void ParseSM002(string pBuffer, Action<Model.ClockCorrectionData> pClockCallback)
        {
            int gs = readUBits(pBuffer, 27, 20);
            // int updateInterval = readUBits(buf, 47, 4);
            // int multipleMessageIndicator = readUBits(buf, 51, 1);
            // int updateIntervalClass = readUBits(buf, 52, 1);
            // int reserved = readUBits(buf, 53, 4);
            int gnssIndicator = readUBits(pBuffer, 57, 4);
            int noSat = readUBits(pBuffer, 61, 6);

            int pos = 67;
            for (int i = 0; i < noSat; i++)
            {
                int prn = _prnHeader(gnssIndicator) + readUBits(pBuffer, pos, 6);
                double c0 = readBits(pBuffer, pos + 6, 22) * 0.0001;
                double c1 = readBits(pBuffer, pos + 28, 21) * 0.000001;
                double c2 = readBits(pBuffer, pos + 49, 27) * 0.00000002;

                pClockCallback(new ClockCorrectionData()
                {
                    gs = gs,
                    prn = prn,
                    deltaClockC0 = c0,
                    deltaClockC1 = c1,
                    deltaClockC2 = c2,
                });
                pos += 76;
            }
        }
        // satb
        public static void parseSM003(string pBuffer, Action<Model.SignalBiasData> pSignalCallback)
        {
            int ver = readUBits(pBuffer, 24, 3);
            int gs = readUBits(pBuffer, 27, 20);
            // int updateInterval = readUBits(buf, 47, 4);
            // int multipleMessageIndicator = readUBits(buf, 51, 1);
            // int updateIntervalClass = readUBits(buf, 52, 1);
            // int reserved = readUBits(buf, 53, 4);
            int gnssIndicator = readUBits(pBuffer, 57, 4);
            int noSat = readUBits(pBuffer, 61, 6);

            int pos = 67;
            for (int i = 0; i < noSat; i++)
            {
                int prn = _prnHeader(gnssIndicator) + readUBits(pBuffer, pos, 6);
                if (ver == 0)
                {
                    double c1 = readBits(pBuffer, pos + 6, 14) * 0.01;
                    double p2 = readBits(pBuffer, pos + 20, 14) * 0.01;
                    double l1 = readBits(pBuffer, pos + 34, 22) * 0.0001;
                    double l2 = readBits(pBuffer, pos + 56, 22) * 0.0001;

                    pSignalCallback(new SignalBiasData()
                    {
                        gs = gs,
                        prn = prn,
                        signalIndex = 0,
                        codeBias = c1,
                        phaseBias = l1,
                    });

                    pSignalCallback(new SignalBiasData()
                    {
                        gs = gs,
                        prn = prn,
                        signalIndex = 11,
                        codeBias = p2,
                        phaseBias = l2,
                    });
                    pos += 78;
                }
                else
                {
                    int noSigs = readUBits(pBuffer, pos + 6, 4);
                    pos += (ver == 1) ? 10 : 11;

                    for (int j = 0; j < noSigs; j++)
                    {
                        int id = readUBits(pBuffer, pos, 5);
                        double c = readBits(pBuffer, pos + 5, 14) * 0.01;
                        double p = readBits(pBuffer, pos + 19, 22) * 0.0001;

                        pSignalCallback(new SignalBiasData()
                        {
                            gs = gs,
                            prn = prn,
                            signalIndex = id,
                            codeBias = c,
                            phaseBias = p,
                        });

                        pos += (ver <= 2) ? 45 : 55;
                    }
                }
            }
        }
        // trop
        public static void parseSM004(string pBuffer, Action<Model.TroposphereDelayData> pTroposphereCallback)
        {
            int ver = readUBits(pBuffer, 24, 3);
            int gs = readUBits(pBuffer, 27, 20);
            // int updateInterval = readUBits(buf, 47, 4);
            // int multipleMessageIndicator = readUBits(buf, 51, 1);
            // int updateIntervalClass = readUBits(buf, 52, 1);
            // int reserved = readUBits(buf, 53, 4);
            int noGP = readUBits(pBuffer, 57, 6);

            int pos = 63;
            for (int i = 0; i < noGP; i++)
            {
                if (ver <= 1)
                {
                    // int type = readUBits(buf, pos, 2);
                    double lat = readBits(pBuffer, pos + 2, 28) * 0.000001;
                    double lon = readBits(pBuffer, pos + 30, 29) * 0.000001;
                    double hgt = readBits(pBuffer, pos + 59, 24) * 0.001;
                    double tr = readBits(pBuffer, pos + 83, 16) * 0.0001;
                    double tw = readBits(pBuffer, pos + 99, 14) * 0.0001;

                    pTroposphereCallback(new TroposphereDelayData()
                    {
                        gs = gs,
                        latitude = (int)lat,
                        longitude = (int)lon,
                        height = (int)hgt,
                        ztd = tr,
                        zwd = tw,
                    });
                    pos += 113;
                }
                else
                {
                    // int type = readUBits(buf, pos, 2);
                    double lat = readBits(pBuffer, pos + 2, 28) * 0.000001;
                    double lon = readBits(pBuffer, pos + 30, 29) * 0.000001;
                    double hgt = readBits(pBuffer, pos + 59, 24) * 0.001;
                    double tr = readBits(pBuffer, pos + 83, 18) * 0.0001;
                    double tw = readBits(pBuffer, pos + 101, 14) * 0.0001;

                    pTroposphereCallback(new TroposphereDelayData()
                    {
                        gs = gs,
                        latitude = (int)lat,
                        longitude = (int)lon,
                        height = (int)hgt,
                        ztd = tr,
                        zwd = tw,
                    });
                    pos += 115;
                }
            }
        }
        // stec
        public static void parseSM005(string pBuffer, Action<Model.IonosphereDelayData> pIonospherCallback)
        {
            int gs = readUBits(pBuffer, 27, 20);
            // int updateInterval = readUBits(buf, 47, 4);
            // int multipleMessageIndicator = readUBits(buf, 51, 1);
            // int updateIntervalClass = readUBits(buf, 52, 1);
            // int reserved = readUBits(buf, 53, 4);
            int gnssIndicator = readUBits(pBuffer, 57, 4);
            int noGP = readUBits(pBuffer, 61, 6);

            int pos = 67;
            for (int i = 0; i < noGP; i++)
            {
                // int type = readUBits(buf, pos, 2);
                double lat = readBits(pBuffer, pos + 2, 28) * 0.000001;
                double lon = readBits(pBuffer, pos + 30, 29) * 0.000001;
                double hgt = readBits(pBuffer, pos + 59, 24) * 0.001;
                int noSat = readUBits(pBuffer, pos + 83, 6);
                pos += 89;

                for (int j = 0; j < noSat; j++)
                {
                    int prn = _prnHeader(gnssIndicator) + readUBits(pBuffer, pos, 6);
                    double stecValue = readBits(pBuffer, pos + 6, 30) * 0.00001;
                    pos += 36;

                    pIonospherCallback(new IonosphereDelayData()
                    {
                        gs = gs,
                        latitude = (int)lat,
                        longitude = (int)lon,
                        height = (int)hgt,
                        prn = prn,
                        stec = stecValue,
                    });
                }
            }
        }
        // trop stec
        public static void parseSM006(string pBuffer, Action<Model.TroposphereDelayData> pTroposphereCallback, Action<Model.IonosphereDelayData> pIonospherCallback)
        {
            int ver = readUBits(pBuffer, 24, 3);
            int gs = readUBits(pBuffer, 27, 20);
            // int updateInterval = readUBits(buf, 47, 4);
            // int multipleMessageIndicator = readUBits(buf, 51, 1);
            // int updateIntervalClass = readUBits(buf, 52, 1);
            // int reserved = readUBits(buf, 53, 4);
            int gnssIndicator = readUBits(pBuffer, 57, 4);
            int noGP = readUBits(pBuffer, 61, 6);


            int pos = 67;
            // int type = 0;
            double lat;
            double lon;
            double hgt;
            double tr;
            double tw;
            double noSat;
            int prn;
            double stecValue;
            for (int i = 0; i < noGP; i++)
            {
                if (ver <= 1)
                {
                    // type = readUBits(buf, pos, 2);
                    lat = readBits(pBuffer, pos + 2, 28) * 0.000001;
                    lon = readBits(pBuffer, pos + 30, 29) * 0.000001;
                    hgt = readBits(pBuffer, pos + 59, 24) * 0.001;
                    tr = readBits(pBuffer, pos + 83, 16) * 0.0001;
                    tw = readBits(pBuffer, pos + 99, 14) * 0.0001;
                    noSat = readUBits(pBuffer, pos + 113, 6);
                    pos += 119;
                }
                else
                {
                    // type = readUBits(buf, pos, 2);
                    lat = readBits(pBuffer, pos + 2, 28) * 0.000001;
                    lon = readBits(pBuffer, pos + 30, 29) * 0.000001;
                    hgt = readBits(pBuffer, pos + 59, 24) * 0.001;
                    tr = readBits(pBuffer, pos + 83, 18) * 0.0001;
                    tw = readBits(pBuffer, pos + 101, 14) * 0.0001;
                    noSat = readUBits(pBuffer, pos + 115, 6);
                    pos += 121;
                }

                pTroposphereCallback(new TroposphereDelayData()
                {
                    gs = gs,
                    latitude = (int)lat,
                    longitude = (int)lon,
                    height = (int)hgt,
                    ztd = tr,
                    zwd = tw,
                });

                for (int j = 0; j < noSat; j++)
                {
                    prn = _prnHeader(gnssIndicator) + readUBits(pBuffer, pos, 6);
                    stecValue = readBits(pBuffer, pos + 6, 30) * 0.00001;
                    pos += 36;

                    pIonospherCallback(new IonosphereDelayData()
                    {
                        gs = gs,
                        latitude = (int)lat,
                        longitude = (int)lon,
                        height = (int)hgt,
                        prn = prn,
                        stec = stecValue,
                    });
                }
            }
        }

        private static int _prnHeader(int gnssIndicator)
        {
            switch (gnssIndicator)
            {
                case 0:
                    return 100;
                case 1:
                    return 300;
                case 2:
                    return 400;
                case 3:
                    return 600;
                case 4:
                    return 500;
                case 5:
                    return 200;
            }
            return 0;
        }

        private static int readUBits(string pBuffer, int pIndex, int pLength)
        {
            if (pLength > 20)
            {
                return -1;
            }

            string bits = pBuffer.Substring(pIndex, pLength);
            return Convert.ToInt32(bits, 2);
        }

        private static int readBits(string pBuffer, int pIndex, int pLength)
        {
            if (pLength > 32)
            {
                return -1;
            }

            if (pLength == 1)
            {
                string bitsStr = pBuffer.Substring(pIndex, pLength);
                return Convert.ToInt32(bitsStr, 2);
            }

            string signBitsStr = pBuffer.Substring(pIndex, 1);
            string valueBitsStr = pBuffer.Substring(pIndex + 1, pLength - 1);

            int signBit = Convert.ToInt32(signBitsStr, 2);
            int value = Convert.ToInt32(valueBitsStr, 2);

            signBit = signBit << pLength - 1;

            if (pLength == 32)
            {
                value = value + signBit;
            }
            else
            {
                value = value - signBit;
            }
            return value;
        }
    }
}
