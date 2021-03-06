""" Python Character Mapping Codec generated from 'CP874.TXT' with gencodec.py.

Written by Marc-Andre Lemburg (mal@lemburg.com).

(c) Copyright CNRI, All Rights Reserved. NO WARRANTY.
(c) Copyright 2000 Guido van Rossum.

"""#"

import codecs

### Codec APIs

class Codec(codecs.Codec):

    def encode(self,input,errors='strict'):

        return codecs.charmap_encode(input,errors,encoding_map)

    def decode(self,input,errors='strict'):

        return codecs.charmap_decode(input,errors,decoding_map)

class StreamWriter(Codec,codecs.StreamWriter):
    pass

class StreamReader(Codec,codecs.StreamReader):
    pass

### encodings module API

def getregentry():

    return (Codec().encode,Codec().decode,StreamReader,StreamWriter)

### Decoding Map

decoding_map = codecs.make_identity_dict(range(256))
decoding_map.update({
        0x0080: 0x20ac, # EURO SIGN
        0x0081: None,   # UNDEFINED
        0x0082: None,   # UNDEFINED
        0x0083: None,   # UNDEFINED
        0x0084: None,   # UNDEFINED
        0x0085: 0x2026, # HORIZONTAL ELLIPSIS
        0x0086: None,   # UNDEFINED
        0x0087: None,   # UNDEFINED
        0x0088: None,   # UNDEFINED
        0x0089: None,   # UNDEFINED
        0x008a: None,   # UNDEFINED
        0x008b: None,   # UNDEFINED
        0x008c: None,   # UNDEFINED
        0x008d: None,   # UNDEFINED
        0x008e: None,   # UNDEFINED
        0x008f: None,   # UNDEFINED
        0x0090: None,   # UNDEFINED
        0x0091: 0x2018, # LEFT SINGLE QUOTATION MARK
        0x0092: 0x2019, # RIGHT SINGLE QUOTATION MARK
        0x0093: 0x201c, # LEFT DOUBLE QUOTATION MARK
        0x0094: 0x201d, # RIGHT DOUBLE QUOTATION MARK
        0x0095: 0x2022, # BULLET
        0x0096: 0x2013, # EN DASH
        0x0097: 0x2014, # EM DASH
        0x0098: None,   # UNDEFINED
        0x0099: None,   # UNDEFINED
        0x009a: None,   # UNDEFINED
        0x009b: None,   # UNDEFINED
        0x009c: None,   # UNDEFINED
        0x009d: None,   # UNDEFINED
        0x009e: None,   # UNDEFINED
        0x009f: None,   # UNDEFINED
        0x00a1: 0x0e01, # THAI CHARACTER KO KAI
        0x00a2: 0x0e02, # THAI CHARACTER KHO KHAI
        0x00a3: 0x0e03, # THAI CHARACTER KHO KHUAT
        0x00a4: 0x0e04, # THAI CHARACTER KHO KHWAI
        0x00a5: 0x0e05, # THAI CHARACTER KHO KHON
        0x00a6: 0x0e06, # THAI CHARACTER KHO RAKHANG
        0x00a7: 0x0e07, # THAI CHARACTER NGO NGU
        0x00a8: 0x0e08, # THAI CHARACTER CHO CHAN
        0x00a9: 0x0e09, # THAI CHARACTER CHO CHING
        0x00aa: 0x0e0a, # THAI CHARACTER CHO CHANG
        0x00ab: 0x0e0b, # THAI CHARACTER SO SO
        0x00ac: 0x0e0c, # THAI CHARACTER CHO CHOE
        0x00ad: 0x0e0d, # THAI CHARACTER YO YING
        0x00ae: 0x0e0e, # THAI CHARACTER DO CHADA
        0x00af: 0x0e0f, # THAI CHARACTER TO PATAK
        0x00b0: 0x0e10, # THAI CHARACTER THO THAN
        0x00b1: 0x0e11, # THAI CHARACTER THO NANGMONTHO
        0x00b2: 0x0e12, # THAI CHARACTER THO PHUTHAO
        0x00b3: 0x0e13, # THAI CHARACTER NO NEN
        0x00b4: 0x0e14, # THAI CHARACTER DO DEK
        0x00b5: 0x0e15, # THAI CHARACTER TO TAO
        0x00b6: 0x0e16, # THAI CHARACTER THO THUNG
        0x00b7: 0x0e17, # THAI CHARACTER THO THAHAN
        0x00b8: 0x0e18, # THAI CHARACTER THO THONG
        0x00b9: 0x0e19, # THAI CHARACTER NO NU
        0x00ba: 0x0e1a, # THAI CHARACTER BO BAIMAI
        0x00bb: 0x0e1b, # THAI CHARACTER PO PLA
        0x00bc: 0x0e1c, # THAI CHARACTER PHO PHUNG
        0x00bd: 0x0e1d, # THAI CHARACTER FO FA
        0x00be: 0x0e1e, # THAI CHARACTER PHO PHAN
        0x00bf: 0x0e1f, # THAI CHARACTER FO FAN
        0x00c0: 0x0e20, # THAI CHARACTER PHO SAMPHAO
        0x00c1: 0x0e21, # THAI CHARACTER MO MA
        0x00c2: 0x0e22, # THAI CHARACTER YO YAK
        0x00c3: 0x0e23, # THAI CHARACTER RO RUA
        0x00c4: 0x0e24, # THAI CHARACTER RU
        0x00c5: 0x0e25, # THAI CHARACTER LO LING
        0x00c6: 0x0e26, # THAI CHARACTER LU
        0x00c7: 0x0e27, # THAI CHARACTER WO WAEN
        0x00c8: 0x0e28, # THAI CHARACTER SO SALA
        0x00c9: 0x0e29, # THAI CHARACTER SO RUSI
        0x00ca: 0x0e2a, # THAI CHARACTER SO SUA
        0x00cb: 0x0e2b, # THAI CHARACTER HO HIP
        0x00cc: 0x0e2c, # THAI CHARACTER LO CHULA
        0x00cd: 0x0e2d, # THAI CHARACTER O ANG
        0x00ce: 0x0e2e, # THAI CHARACTER HO NOKHUK
        0x00cf: 0x0e2f, # THAI CHARACTER PAIYANNOI
        0x00d0: 0x0e30, # THAI CHARACTER SARA A
        0x00d1: 0x0e31, # THAI CHARACTER MAI HAN-AKAT
        0x00d2: 0x0e32, # THAI CHARACTER SARA AA
        0x00d3: 0x0e33, # THAI CHARACTER SARA AM
        0x00d4: 0x0e34, # THAI CHARACTER SARA I
        0x00d5: 0x0e35, # THAI CHARACTER SARA II
        0x00d6: 0x0e36, # THAI CHARACTER SARA UE
        0x00d7: 0x0e37, # THAI CHARACTER SARA UEE
        0x00d8: 0x0e38, # THAI CHARACTER SARA U
        0x00d9: 0x0e39, # THAI CHARACTER SARA UU
        0x00da: 0x0e3a, # THAI CHARACTER PHINTHU
        0x00db: None,   # UNDEFINED
        0x00dc: None,   # UNDEFINED
        0x00dd: None,   # UNDEFINED
        0x00de: None,   # UNDEFINED
        0x00df: 0x0e3f, # THAI CURRENCY SYMBOL BAHT
        0x00e0: 0x0e40, # THAI CHARACTER SARA E
        0x00e1: 0x0e41, # THAI CHARACTER SARA AE
        0x00e2: 0x0e42, # THAI CHARACTER SARA O
        0x00e3: 0x0e43, # THAI CHARACTER SARA AI MAIMUAN
        0x00e4: 0x0e44, # THAI CHARACTER SARA AI MAIMALAI
        0x00e5: 0x0e45, # THAI CHARACTER LAKKHANGYAO
        0x00e6: 0x0e46, # THAI CHARACTER MAIYAMOK
        0x00e7: 0x0e47, # THAI CHARACTER MAITAIKHU
        0x00e8: 0x0e48, # THAI CHARACTER MAI EK
        0x00e9: 0x0e49, # THAI CHARACTER MAI THO
        0x00ea: 0x0e4a, # THAI CHARACTER MAI TRI
        0x00eb: 0x0e4b, # THAI CHARACTER MAI CHATTAWA
        0x00ec: 0x0e4c, # THAI CHARACTER THANTHAKHAT
        0x00ed: 0x0e4d, # THAI CHARACTER NIKHAHIT
        0x00ee: 0x0e4e, # THAI CHARACTER YAMAKKAN
        0x00ef: 0x0e4f, # THAI CHARACTER FONGMAN
        0x00f0: 0x0e50, # THAI DIGIT ZERO
        0x00f1: 0x0e51, # THAI DIGIT ONE
        0x00f2: 0x0e52, # THAI DIGIT TWO
        0x00f3: 0x0e53, # THAI DIGIT THREE
        0x00f4: 0x0e54, # THAI DIGIT FOUR
        0x00f5: 0x0e55, # THAI DIGIT FIVE
        0x00f6: 0x0e56, # THAI DIGIT SIX
        0x00f7: 0x0e57, # THAI DIGIT SEVEN
        0x00f8: 0x0e58, # THAI DIGIT EIGHT
        0x00f9: 0x0e59, # THAI DIGIT NINE
        0x00fa: 0x0e5a, # THAI CHARACTER ANGKHANKHU
        0x00fb: 0x0e5b, # THAI CHARACTER KHOMUT
        0x00fc: None,   # UNDEFINED
        0x00fd: None,   # UNDEFINED
        0x00fe: None,   # UNDEFINED
        0x00ff: None,   # UNDEFINED
})

### Encoding Map

encoding_map = codecs.make_encoding_map(decoding_map)
