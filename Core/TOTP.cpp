// Using for generate and verify TOTP Code

#include "pch.h"
using namespace std;


static string Base32Encode(const vector<uint8_t>& data) {
    static const char* ALPH = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
    string out;
    int bits = 0;
    uint32_t buffer = 0;
    for (uint8_t b : data) {
        buffer = (buffer << 8) | b;
        bits += 8;
        while (bits >= 5) {
            out.push_back(ALPH[(buffer >> (bits - 5)) & 0x1F]);
            bits -= 5;
        }
    }
    if (bits > 0) {
        out.push_back(ALPH[(buffer << (5 - bits)) & 0x1F]);
    }
    return out;
}

static string UrlEncode(const string& s) {
    ostringstream oss;
    for (unsigned char c : s) {
        if ((c >= 'A' && c <= 'Z') ||
            (c >= 'a' && c <= 'z') ||
            (c >= '0' && c <= '9') ||
            c == '-' || c == '_' || c == '.' || c == '~') {
            oss << c;
        }
        else {
            oss << '%' << uppercase << hex
                << setw(2) << setfill('0') << int(c)
                << nouppercase << dec;
        }
    }
    return oss.str();
}

struct Sha1 {
    uint32_t h0 = 0x67452301, h1 = 0xEFCDAB89, h2 = 0x98BADCFE, h3 = 0x10325476, h4 = 0xC3D2E1F0;
    uint64_t total_bits = 0;
    uint8_t buffer[64]{};
    size_t buf_len = 0;

    static uint32_t rol(uint32_t x, uint32_t n) { return (x << n) | (x >> (32 - n)); }

    void process_block(const uint8_t b[64]) {
        uint32_t w[80];
        for (int i = 0; i < 16; ++i) {
            w[i] = (uint32_t(b[4 * i]) << 24) | (uint32_t(b[4 * i + 1]) << 16) |
                (uint32_t(b[4 * i + 2]) << 8) | (uint32_t(b[4 * i + 3]));
        }
        for (int i = 16; i < 80; ++i) w[i] = rol(w[i - 3] ^ w[i - 8] ^ w[i - 14] ^ w[i - 16], 1);

        uint32_t a = h0, bv = h1, c = h2, d = h3, e = h4;
        for (int i = 0; i < 80; ++i) {
            uint32_t f, k;
            if (i < 20) { f = (bv & c) | ((~bv) & d); k = 0x5A827999; }
            else if (i < 40) { f = bv ^ c ^ d; k = 0x6ED9EBA1; }
            else if (i < 60) { f = (bv & c) | (bv & d) | (c & d); k = 0x8F1BBCDC; }
            else { f = bv ^ c ^ d; k = 0xCA62C1D6; }
            uint32_t temp = rol(a, 5) + f + e + k + w[i];
            e = d; d = c; c = rol(bv, 30); bv = a; a = temp;
        }
        h0 += a; h1 += bv; h2 += c; h3 += d; h4 += e;
    }

    void update(const uint8_t* data, size_t len) {
        total_bits += uint64_t(len) * 8;
        size_t i = 0;
        if (buf_len) {
            while (buf_len < 64 && i < len) buffer[buf_len++] = data[i++];
            if (buf_len == 64) { process_block(buffer); buf_len = 0; }
        }
        while (i + 64 <= len) { process_block(data + i); i += 64; }
        while (i < len) buffer[buf_len++] = data[i++];
    }

    vector<uint8_t> finalize() {
        uint8_t pad[64] = { 0x80 };
        size_t pad_len = (buf_len < 56) ? (56 - buf_len) : (56 + 64 - buf_len);
        update(pad, pad_len);
        uint8_t len_bytes[8];
        for (int i = 0; i < 8; ++i) len_bytes[7 - i] = uint8_t((total_bits >> (8 * i)) & 0xFF);
        update(len_bytes, 8);
        vector<uint8_t> out(20);
        uint32_t hs[5] = { h0,h1,h2,h3,h4 };
        for (int i = 0; i < 5; ++i) {
            out[4 * i + 0] = uint8_t((hs[i] >> 24) & 0xFF);
            out[4 * i + 1] = uint8_t((hs[i] >> 16) & 0xFF);
            out[4 * i + 2] = uint8_t((hs[i] >> 8) & 0xFF);
            out[4 * i + 3] = uint8_t((hs[i]) & 0xFF);
        }
        return out;
    }
};

static void HmacSha1(const uint8_t* key, size_t key_len, const uint8_t* msg, size_t msg_len, uint8_t out[20]) {
    uint8_t k0[64]; memset(k0, 0, 64);
    if (key_len > 64) {
        Sha1 s; s.update(key, key_len);
        auto d = s.finalize();
        memcpy(k0, d.data(), 20);
    }
    else {
        memcpy(k0, key, key_len);
    }
    uint8_t ipad[64], opad[64];
    for (size_t i = 0; i < 64; ++i) { ipad[i] = k0[i] ^ 0x36; opad[i] = k0[i] ^ 0x5c; }
    Sha1 si; si.update(ipad, 64); si.update(msg, msg_len);
    auto inner = si.finalize();
    Sha1 so; so.update(opad, 64); so.update(inner.data(), inner.size());
    auto mac = so.finalize();
    memcpy(out, mac.data(), 20);
}

static string HotpCode(const uint8_t* key, size_t key_len, uint64_t counter, int digits) {
    uint8_t msg[8];
    for (int i = 7; i >= 0; --i) { msg[i] = uint8_t(counter & 0xFF); counter >>= 8; }
    uint8_t mac[20]; HmacSha1(key, key_len, msg, 8, mac);
    int offset = mac[19] & 0x0F;
    uint32_t bin = ((mac[offset] & 0x7F) << 24) |
        ((mac[offset + 1] & 0xFF) << 16) |
        ((mac[offset + 2] & 0xFF) << 8) |
        ((mac[offset + 3] & 0xFF));
    uint32_t mod = 1;
    for (int i = 0; i < digits; ++i) mod *= 10;
    uint32_t val = bin % mod;
    ostringstream oss; oss << setw(digits) << setfill('0') << val;
    return oss.str();
}

EXPORT_DLL BSTR CreateTOTPUrl() 
{
	wcout << L"IslandCaller.Core | Info | Start create TOTP secret and url\n";
    std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter; // UTF-8 => UTF-16
    constexpr LPCWSTR REG_PATH = L"SOFTWARE\\IslandCaller\\Security\\SecretKey";
    constexpr LPCWSTR REG_VALUE = L"TOTPKey";
    vector<uint8_t> secret(20);
    random_device rd;
    for (auto& b : secret) b = static_cast<uint8_t>(rd());
    // 固定参数
    const string issuer = "IslandCaller";
    const string account = "Administrator";
    const int digits = 6;
    const int period = 30;

    string secret_b32 = Base32Encode(secret);
    ostringstream oss;
    oss << "otpauth://totp/"
        << UrlEncode(issuer) << ":" << UrlEncode(account)
        << "?secret=" << secret_b32
        << "&issuer=" << UrlEncode(issuer)
        << "&algorithm=SHA1"
        << "&digits=" << digits
        << "&period=" << period;
   
    HKEY hKey;
    LONG res = RegCreateKeyExW(HKEY_CURRENT_USER, REG_PATH, 0, NULL,
        REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &hKey, NULL);
    if (res != ERROR_SUCCESS) {
        wcout << "IslandCaller.Core | Error | RegCreateKeyEx failed: " << res << "\n";
        return SysAllocString(converter.from_bytes("").c_str());
    }

    res = RegSetValueExW(hKey, REG_VALUE, 0, REG_BINARY,
        reinterpret_cast<const BYTE*>(secret.data()),
        static_cast<DWORD>(secret.size()));
    RegCloseKey(hKey);

    if (res != ERROR_SUCCESS) {
        std::cerr << "IslandCaller.Core | Error | RegSetValueEx failed: " << res << "\n";
        return SysAllocString(converter.from_bytes("").c_str());
    }
	wcout << L"IslandCaller.Core | Success | TOTP secret and url created successfully\n";
	wcout << L"IslandCaller.Core | Info | TOTP URL: " << converter.from_bytes(oss.str()) << L"\n";
    wcout << L"IslandCaller.Core | Debug | Secret (Base32): " << converter.from_bytes(secret_b32) << L"\n";
    wcout << L"IslandCaller.Core | Debug | Secret (Hex): ";
    for (auto b : secret) wcout << hex << setw(2) << setfill(L'0') << int(b);
    wcout << endl;
    return SysAllocString(converter.from_bytes(oss.str()).c_str());
}

EXPORT_DLL bool VerifyTOTP(const wchar_t* user_code)
{
	wcout << L"IslandCaller.Core | Info | Start verify TOTP code\n";
    wstring_convert<codecvt_utf8<wchar_t>> conv;
    string usercode_utf8 = conv.to_bytes(user_code);
    wcout << L"IslandCaller.Core | Info | User provided code: " << user_code << L"\n";
    constexpr LPCWSTR REG_PATH = L"SOFTWARE\\IslandCaller\\Security\\SecretKey";
    constexpr LPCWSTR REG_VALUE = L"TOTPKey";

    HKEY hKey;
    LONG res = RegOpenKeyExW(HKEY_CURRENT_USER, REG_PATH, 0, KEY_READ, &hKey);
    if (res != ERROR_SUCCESS) {
		wcout << L"IslandCaller.Core | Error | RegOpenKeyEx failed: " << res << L"\n";
		return false;
    }

    DWORD type = 0;
    DWORD dataSize = 0;
    res = RegGetValueW(hKey, NULL, REG_VALUE, RRF_RT_REG_BINARY, &type, NULL, &dataSize);
    if (res != ERROR_SUCCESS || type != REG_BINARY) {
        RegCloseKey(hKey);
		wcout << L"IslandCaller.Core | Error | RegGetValue size query failed or wrong type: " << res << L"\n";
		return false;
    }

    std::vector<uint8_t> secret(dataSize);
    res = RegGetValueW(hKey, NULL, REG_VALUE, RRF_RT_REG_BINARY, NULL, secret.data(), &dataSize);
    RegCloseKey(hKey);

    if (res != ERROR_SUCCESS) {
		wcout << L"IslandCaller.Core | Error | RegGetValue data read failed: " << res << L"\n";
		return false;
    }

    // 固定参数
    const int digits = 6;
    const int period = 30;
    const int window = 1; // 容忍时间步偏移

    using namespace chrono;
    uint64_t now = duration_cast<seconds>(system_clock::now().time_since_epoch()).count();
    uint64_t counter = now / period;

    for (int delta = -window; delta <= window; ++delta) {
        int64_t c = int64_t(counter) + delta;
        if (c < 0) continue;
        string code = HotpCode(secret.data(), secret.size(), uint64_t(c), digits);
        wcout << L"IslandCaller.Core | Info | Generated code for counter " << c << L": " << conv.from_bytes(code) << L"\n";
        if (code == usercode_utf8) {
            wcout << L"IslandCaller.Core | Success | TOTP code verified successfully\n";
            return true;
        }
    }
	wcout << L"IslandCaller.Core | Failed | TOTP code wrong\n";
    return false;
}
