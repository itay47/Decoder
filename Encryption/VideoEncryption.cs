namespace Decoder.Encryption
{
    public class VideoEncryption
    {
        //private static bool useV1 = true;

        private static int currentClipReadCrypto;
        private static string string1V2 = "pluralsight";
        private static string string2V2 = "#©>Å£Q\x0005¤°";
        private const string String1 = "pluralsight";
        private const string String2 = "\x0006?zY¢\x00B2\x0085\x009FL\x00BEî0Ö.ì\x0017#©>Å£Q\x0005¤°\x00018Þ^\x008Eú\x0019Lqß'\x009D\x0003ßE\x009EM\x0080'x:\0~\x00B9\x0001ÿ 4\x00B3õ\x0003Ã§Ê\x000EAË\x00BC\x0090è\x009Eî~\x008B\x009Aâ\x001B¸UD<\x007FKç*\x001Döæ7H\v\x0015Arý*v÷%Âþ\x00BEä;pü";
        internal static readonly string[][] CryptoKeys = new string[3][]
        {
            new string[2]
            {
              "pluralsight",
              "\x0006?zY¢\x00B2\x0085\x009FL\x00BEî0Ö.ì\x0017#©>Å£Q\x0005¤°\x00018Þ^\x008Eú\x0019Lqß'\x009D\x0003ßE\x009EM\x0080'x:\0~\x00B9\x0001ÿ 4\x00B3õ\x0003Ã§Ê\x000EAË\x00BC\x0090è\x009Eî~\x008B\x009Aâ\x001B¸UD<\x007FKç*\x001Döæ7H\v\x0015Arý*v÷%Âþ\x00BEä;pü"
            },
            new string[2]
            {
              VideoEncryption.String1V2,
              VideoEncryption.String2V2
            },
            new string[1]
            {
              "os22$!sKHyy9jnGlgHB&vP21CK96tx!l2uhK1K%Fbubree9%o0wT44zwvJ446iAdA%M!@RopKCmOWMOqTt1*BIw@lF68x3itctw"
            }
        };


        public static string String1V2
        {
            get => VideoEncryption.string1V2;
            set
            {
                VideoEncryption.CryptoKeys[1][0] = value;
                VideoEncryption.string1V2 = value;
            }
        }

        public static string String2V2
        {
            get => VideoEncryption.string2V2;
            set
            {
                VideoEncryption.CryptoKeys[1][1] = value;
                VideoEncryption.string2V2 = value;
            }
        }

        private static void XorBuffer(byte[] buff, int length, long position) => VideoEncryption.XorBuffer(VideoEncryption.currentClipReadCrypto, buff, length, position);

        internal static void XorBuffer(int key, byte[] buff, int length, long position)
        {
            for (int index1 = 0; index1 < length; ++index1)
            {
                string[] cryptoKey = VideoEncryption.CryptoKeys[key];
                string str1 = cryptoKey[0];
                int num1 = (int)position + index1;
                char ch = str1[num1 % str1.Length];
                for (int index2 = 1; index2 < cryptoKey.Length; ++index2)
                {
                    string str2 = cryptoKey[index2];
                    ch ^= str2[num1 % str2.Length];
                }
                int num2 = (int)ch ^ num1 % 251;
                buff[index1] = (byte)((uint)buff[index1] ^ (uint)num2);
            }
        }

        public static void EncryptBuffer(byte[] buff, int length, long position) => VideoEncryption.XorBuffer(VideoEncryption.CryptoKeys.Length - 1, buff, length, position);

        public static void DecryptBuffer(byte[] buff, int length, long position)
        {
            if (position == 0L)
            {
                for (int index1 = VideoEncryption.CryptoKeys.Length - 1; index1 >= 0; --index1)
                {
                    VideoEncryption.currentClipReadCrypto = index1;
                    VideoEncryption.XorBuffer(buff, length, position);
                    bool flag = (uint)buff.Length > 0U;
                    for (int index2 = 0; index2 < buff.Length && index2 < 3; ++index2)
                        flag = flag && buff[index2] == (byte)0;
                    if (flag)
                        return;
                    VideoEncryption.XorBuffer(buff, length, position);
                }
            }
            VideoEncryption.XorBuffer(buff, length, position);
        }







        //public static void DecryptBuffer(byte[] buff, int length, long position)
        //{
        //    if ((position != 0) || (length <= 3))
        //    {
        //        if (useV1)
        //        {
        //            XorBuffer(buff, length, position);
        //        }
        //        else
        //        {
        //            XorBufferV2(buff, length, position);
        //        }
        //    }
        //    else
        //    {
        //        XorBuffer(buff, length, position);

        //        if ((buff[0] == 0) && ((buff[1] == 0) && (buff[2] == 0)))
        //        {
        //            useV1 = true;
        //        }
        //        else
        //        {
        //            XorBuffer(buff, length, position);
        //            XorBufferV2(buff, length, position);

        //            useV1 = false;
        //        }
        //    }
        //}

        //public static void XorBuffer(byte[] buff, int length, long position)
        //{
        //    string str = "pluralsight";
        //    string str2 = "\x0006?zY\x00a2\x00b2\x0085\x009fL\x00be\x00ee0\x00d6.\x00ec\x0017#\x00a9>\x00c5\x00a3Q\x0005\x00a4\x00b0\x00018\x00de^\x008e\x00fa\x0019Lq\x00df'\x009d\x0003\x00dfE\x009eM\x0080'x:\0~\x00b9\x0001\x00ff 4\x00b3\x00f5\x0003\x00c3\x00a7\x00ca\x000eA\x00cb\x00bc\x0090\x00e8\x009e\x00ee~\x008b\x009a\x00e2\x001b\x00b8UD<\x007fK\x00e7*\x001d\x00f6\x00e67H\v\x0015Ar\x00fd*v\x00f7%\x00c2\x00fe\x00be\x00e4;p\x00fc";
        //    for (int i = 0; i < length; i++)
        //    {
        //        byte num2 = (byte)((str[(int)((position + i) % ((long)str.Length))] ^ str2[(int)((position + i) % ((long)str2.Length))]) ^ ((position + i) % 0xfbL));
        //        buff[i] = (byte)(buff[i] ^ num2);
        //    }
        //}

        //public static void XorBufferV2(byte[] buff, int length, long position)
        //{
        //    for (int i = 0; i < length; i++)
        //    {
        //        string str = "\0\x00bf{U9\x0001\x00ae`\x00eb\x0013\x00d1[\x001b\x00cf";
        //        string str2 = "\x0002\x008d\a\x0099\x0089\x009a%\x0084K\x00b0s\x00fa\x00c148\x00e4cz@\x009f,\x00ed>\x00f6\x00a02\v\x00df\n@*\x00ed\vz\x008c\x0004\x00bd\x0093\0\x00dce\x00cb\x0086\x001f\b\x00d6\x009e AD\x00d3g&\x00ec\x00b6\x0017\x008d\x00c0\x0014{\x00b5\x00ec\x00df\x0088\x00d8\x009f\x00f2\x00d5\x00c4\x0081p\x00aa\x00aatC\x008a@\x009c2:\x00c5f\\\\\x00ad\x00e8\x009e\x00fd\x0002g\x0003|\x00d8Bf\x0092\x00a0";
        //        byte num2 = (byte)((str[(int)((position + i) % ((long)str.Length))] ^ str2[(int)((position + i) % ((long)str2.Length))]) ^ ((position + i) % 0xfbL));
        //        buff[i] = (byte)(buff[i] ^ num2);
        //    }
        //}
    }
}