using System;

namespace DizProtezApp.Services
{
    public static class PlcRegisters
    {
        // BOOL Adresleri (RegisterAddress, BitIndex)
            // Servo 1
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_Start_INITC = (1100, 0); // CanOpen iletişim başlat biti
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_INIT_OK = (1101, 0); // CanOpen iletişimi başladı biti
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_Start_CASD = (1100, 1); // acc ve dcc zamnlarını servoya yazma biti
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_Servo_ON = (1100, 2); // servoyu aktif et
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_Servo_DURDUR = (1100, 15); // servoyu DURDUR anlık 
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_Servo_RESET = (1100, 9); // servoyu DURDUR anlık 
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_Zrnm_Start = (1100, 3); // servo home modu seçme biti
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_Zrnm_OK = (1101, 1); // servo home modu seçildi
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_Zrnm_Err = (1101, 2); // servo home modu seçilemedi
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_DDRVAC_Start = (1100, 4); // pozisyona git ( absolute)
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_DDRVIC_Start = (1100, 5); // pozisyona git (relative)
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_DPLSVC_Start = (1100, 6); // hız kontrol modu başlat
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_DPLSVC_FWD_Start = (1100, 7); // ileri jog
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_DPLSVC_REV_Start = (1100, 8); // geri jog
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_COPWL_Start = (1100, 10); // can open kominikasyon datası yaz word
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_DCOPWL_Start = (1100, 11); // can open kominikasyon datası yaz dword
                public static readonly (ushort RegisterAddress, byte BitIndex) copwl_ok = (1101, 3); // word data yazıldı
                public static readonly (ushort RegisterAddress, byte BitIndex) dcopwl_ok = (1101, 4); // dword data yazıldı
                public static readonly (ushort RegisterAddress, byte BitIndex) s1_zrnc_Start = (1100, 12); // home ara
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_Home_OK = (1101, 5); // home ok
                public static readonly (ushort RegisterAddress, byte BitIndex) S1_Coprw_Start = (1100, 13); // data yaz-oku start biti
                public static readonly (ushort RegisterAddress, byte BitIndex) s1_coprw_ok = (1101, 6); // data yaz oku ok biti

            // Test Verileri
                public static readonly (ushort RegisterAddress, byte BitIndex) MAN_OTO_SEC_BIT = (20150, 0); // MANUEL OTOMATİK SEÇME BUTONU
                public static readonly (ushort RegisterAddress, byte BitIndex) TEST1_BASLA_DURDUR = (1150, 1); // TEST 1 BAŞLA DURDUR BUTONU
                public static readonly (ushort RegisterAddress, byte BitIndex) TEST2_BASLA_DURDUR = (1150, 2); // TEST 2 BAŞLA DURDUR BUTONU
                public static readonly (ushort RegisterAddress, byte BitIndex) TEST3_BASLA_DURDUR = (1150, 3); // TEST 3 BAŞLA DURDUR BUTONU
                public static readonly (ushort RegisterAddress, byte BitIndex) TEST4_BASLA_DURDUR = (1150, 4); // TEST 4 BAŞLA DURDUR BUTONU
                public static readonly (ushort RegisterAddress, byte BitIndex) TEST5_BASLA_DURDUR = (1150, 5); // TEST 5 BAŞLA DURDUR BUTONU

        // WORD Adresleri
            // Servo 1
                public static readonly ushort S1_Acc_time = 20100; // motor hızlanma rampası
                public static readonly ushort S1_Dec_time = 20102; // motor yavaşlama rampası
                public static readonly ushort S1_First_Speed = 20116;   // home 1. hız
                public static readonly ushort S1_Second_Speed = 20118; // home 2.hız
                public static readonly ushort S1_Write_Data = 1118; // YAZILACAK DATA
                public static readonly ushort S1_servo_hata = 20120; // hata kodu
            // Loadcell 1
                public static readonly ushort LOADCELL_1_AYAR = 20416; // AYAR İÇİN KULLANILACAK
                public static readonly ushort LOADCELL_1_HATA = 20418; // HATA KODU OKUNACAK

        // DWORD Adresleri
            // Servo 1
                public static readonly ushort S1_ManuelHız = 20130; // gidilecek pozisyon
                public static readonly ushort S1_Pozisyon = 20104; // gidilecek pozisyon
                public static readonly ushort S1_Speed = 20106; // gitme hızı
                public static readonly ushort S1_FWD_Speed = 20108; // ileri jog hızı
                public static readonly ushort S1_REV_Speed = 20110; // geri jog hızı
                public static readonly ushort S1_input_counter = 1110; // 
                public static readonly ushort S1_input_speed = 1112; // 
                public static readonly ushort S1_output_counter = 1114; //
                public static readonly ushort S1_output_speed = 1116; // gidilecek pozisyon
                public static readonly ushort S1_Anlık_Poz = 1120; // gidilecek pozisyon
            // TEST SERVO 1
                public static readonly ushort S1_TEST_ILERI_POZ = 20122; // TEST S1 İLERİ POZİSYONU
                public static readonly ushort S1_TEST_GERI_POZ = 20124; // TEST S1 GERİ POZİSYONU
                public static readonly ushort S1_TEST_ILERI_HIZ = 20126; // TEST S1 İLERİ GİDERKEN Kİ HIZ
                public static readonly ushort S1_TEST_GERI_HIZ = 20128; // TEST S1 GERİ GİDERKEN Kİ HIZ
            // TEST SERVO 2
                public static readonly ushort S2_TEST_ILERI_POZ = 20222; // TEST S2 İLERİ POZİSYONU
                public static readonly ushort S2_TEST_GERI_POZ = 20224; // TEST S2 GERİ POZİSYONU
                public static readonly ushort S2_TEST_ILERI_HIZ = 20226; // TEST S2 İLERİ GİDERKEN Kİ HIZ
                public static readonly ushort S2_TEST_GERI_HIZ = 20228; // TEST S2 GERİ GİDERKEN Kİ HIZ
            // TEST SERVO 3
                public static readonly ushort S3_TEST_ILERI_POZ = 20322; // TEST S3 İLERİ POZİSYONU
                public static readonly ushort S3_TEST_GERI_POZ = 20324; // TEST S3 GERİ POZİSYONU
                public static readonly ushort S3_TEST_ILERI_HIZ = 20326; // TEST S3 İLERİ GİDERKEN Kİ HIZ
                public static readonly ushort S3_TEST_GERI_HIZ = 20328; // TEST S3 GERİ GİDERKEN Kİ HIZ
            // Loadcell 1
                public static readonly ushort LOADCELL_1_DWORD = 20414; // servo 1 i ölçen loadcell veri 1 YUKARDAKİ
        public static readonly ushort LOADCELL_2_DWORD = 20424; // servo 1 i ölçen loadcell veri 1 AŞŞAĞIDAKİ

        // REAL Adresleri

        // TEST Loadcell 1
        public static readonly ushort L1_TEST_POZ_LIMIT = 20134; // TEST  L1 POZITIF YÜK LİMİTİ
                public static readonly ushort L1_TEST_NEG_LIMIT = 20136; // TEST  L1 NEGATİF YÜK LİMİTİ
            // TEST Loadcell 2
                public static readonly ushort L2_TEST_POZ_LIMIT = 20134; // TEST  L2 POZITIF YÜK LİMİTİ
                public static readonly ushort L2_TEST_NEG_LIMIT = 20136; // TEST  L2NEGATİF YÜK LİMİTİ
            // TEST Loadcell 3
                public static readonly ushort L3_TEST_POZ_LIMIT = 20134; // TEST  L3 POZITIF YÜK LİMİTİ
                public static readonly ushort L3_TEST_NEG_LIMIT = 20136; // TEST  L3NEGATİF YÜK LİMİTİ

        public static readonly ushort DENEME = 840;

    }
}
