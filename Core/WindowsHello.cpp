#include "pch.h"
#pragma comment(lib, "webauthn.lib")
#pragma comment(lib, "bcrypt.lib")

EXPORT_DLL bool CreateHelloPasskey()
{
    const std::wstring rpId = L"IslandCaller.App"; // 必须与验证时一致
    const std::wstring rpName = L"IslandCaller";

    // 检查 Windows Hello 是否可用
    BOOL available = FALSE;
    if (FAILED(WebAuthNIsUserVerifyingPlatformAuthenticatorAvailable(&available)) || !available)
    {
        std::wcerr << L"[CreatePasskey] Windows Hello Disable\n";
        return false;
    }

    // Relying Party 信息
    WEBAUTHN_RP_ENTITY_INFORMATION rpInfo = { sizeof(rpInfo) };
    rpInfo.pwszId = rpId.c_str();
    rpInfo.pwszName = rpName.c_str();

    // 用户信息
    BYTE userId[16];
    BCryptGenRandom(NULL, userId, sizeof(userId), BCRYPT_USE_SYSTEM_PREFERRED_RNG);

    WEBAUTHN_USER_ENTITY_INFORMATION userInfo = { sizeof(userInfo) };
    userInfo.pbId = userId;
    userInfo.cbId = sizeof(userId);
    userInfo.pwszName = L"Admin@IslandCaller.App";
    userInfo.pwszDisplayName = L"Administrator";

    // 公钥算法参数
    WEBAUTHN_COSE_CREDENTIAL_PARAMETER coseParam = { sizeof(coseParam) };
    coseParam.pwszCredentialType = WEBAUTHN_CREDENTIAL_TYPE_PUBLIC_KEY;
    coseParam.lAlg = WEBAUTHN_COSE_ALGORITHM_ECDSA_P256_WITH_SHA256;
    WEBAUTHN_COSE_CREDENTIAL_PARAMETERS coseParams = { 1, &coseParam };

    // Challenge
    BYTE challenge[32];
    BCryptGenRandom(NULL, challenge, sizeof(challenge), BCRYPT_USE_SYSTEM_PREFERRED_RNG);

    WEBAUTHN_CLIENT_DATA clientData = { sizeof(clientData) };
    clientData.pwszHashAlgId = WEBAUTHN_HASH_ALGORITHM_SHA_256;
    clientData.pbClientDataJSON = challenge;
    clientData.cbClientDataJSON = sizeof(challenge);

    // 创建 Passkey
    WEBAUTHN_CREDENTIAL_ATTESTATION* pAttestation = nullptr;
    HRESULT hr = WebAuthNAuthenticatorMakeCredential(
        GetConsoleWindow(),
        &rpInfo,
        &userInfo,
        &coseParams,
        &clientData,
        nullptr,
        &pAttestation
    );

    if (FAILED(hr) || !pAttestation)
    {
        std::wcerr << L"[CreatePasskey] Failed, HRESULT=0x" << std::hex << hr << L"\n";
        return false;
    }

    // 将 CredentialId 写入注册表
    HKEY hKey;
    if (RegCreateKeyExW(HKEY_CURRENT_USER, L"Software\\IslandCaller\\Security\\SecretKey", 0, nullptr, 0, KEY_WRITE, nullptr, &hKey, nullptr) == ERROR_SUCCESS)
    {
        RegSetValueExW(hKey, L"Passkey", 0, REG_BINARY, pAttestation->pbCredentialId, pAttestation->cbCredentialId);
        RegCloseKey(hKey);
        std::wcout << L"[CreatePasskey] CredentialId Written\n";
    }
    else
    {
        std::wcerr << L"[CreatePasskey] Reg Writing Failed\n";
        WebAuthNFreeCredentialAttestation(pAttestation);
        return false;
    }

    WebAuthNFreeCredentialAttestation(pAttestation);
    return true;
}

EXPORT_DLL bool VerifyHelloPasskey()
{
    // 固定 RP ID（必须与注册时一致）
    const std::wstring rpId = L"IslandCaller.App";

    // 检查 Windows Hello 是否可用
    BOOL available = FALSE;
    if (FAILED(WebAuthNIsUserVerifyingPlatformAuthenticatorAvailable(&available)) || !available)
        return false;

    // 从注册表读取 CredentialId
    std::vector<BYTE> credentialId;
    {
        HKEY hKey;
        if (RegOpenKeyExW(HKEY_CURRENT_USER, L"Software\\IslandCaller\\Security\\SecretKey", 0, KEY_READ, &hKey) != ERROR_SUCCESS)
            return false;

        DWORD type = 0, credSize = 0;
        if (RegQueryValueExW(hKey, L"Passkey", nullptr, &type, nullptr, &credSize) != ERROR_SUCCESS || type != REG_BINARY)
        {
            RegCloseKey(hKey);
            return false;
        }

        credentialId.resize(credSize);
        if (RegQueryValueExW(hKey, L"Passkey", nullptr, &type, credentialId.data(), &credSize) != ERROR_SUCCESS)
        {
            RegCloseKey(hKey);
            return false;
        }
        RegCloseKey(hKey);
    }

    // 生成 Challenge
    BYTE challenge[32];
    if (BCryptGenRandom(NULL, challenge, sizeof(challenge), BCRYPT_USE_SYSTEM_PREFERRED_RNG) != 0)
        return false;

    WEBAUTHN_CLIENT_DATA clientData = { sizeof(clientData) };
    clientData.pwszHashAlgId = WEBAUTHN_HASH_ALGORITHM_SHA_256;
    clientData.pbClientDataJSON = challenge;
    clientData.cbClientDataJSON = sizeof(challenge);

    // 构造允许的凭证
    WEBAUTHN_CREDENTIAL_EX allowCred = { sizeof(allowCred) };
    allowCred.dwVersion = WEBAUTHN_CREDENTIAL_EX_CURRENT_VERSION;
    allowCred.cbId = (DWORD)credentialId.size();
    allowCred.pbId = credentialId.data();
    allowCred.pwszCredentialType = WEBAUTHN_CREDENTIAL_TYPE_PUBLIC_KEY;
    allowCred.dwTransports = WEBAUTHN_CTAP_TRANSPORT_INTERNAL;

    WEBAUTHN_CREDENTIALS allowList = { 1, (PWEBAUTHN_CREDENTIAL)&allowCred };

    // 验证选项（新版结构）
    WEBAUTHN_AUTHENTICATOR_GET_ASSERTION_OPTIONS options = { sizeof(options) };
    options.dwVersion = WEBAUTHN_AUTHENTICATOR_GET_ASSERTION_OPTIONS_CURRENT_VERSION;
    options.dwTimeoutMilliseconds = 60000;
    options.CredentialList = allowList;
    options.dwUserVerificationRequirement = WEBAUTHN_USER_VERIFICATION_REQUIREMENT_REQUIRED;

    // 调用验证 API
    WEBAUTHN_ASSERTION* pAssertion = nullptr;
    HRESULT hr = WebAuthNAuthenticatorGetAssertion(
        GetConsoleWindow(),
        rpId.c_str(),
        &clientData,
        &options,
        &pAssertion
    );

    bool success = SUCCEEDED(hr) && pAssertion;
    if (pAssertion)
        WebAuthNFreeAssertion(pAssertion);

    return success;
}
