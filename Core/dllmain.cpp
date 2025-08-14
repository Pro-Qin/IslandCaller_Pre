#include "pch.h"
using namespace std;

vector<string> students;
bool isAntiRepeat = true;
bool isInitialized = false;
unordered_set<string> RandomHashSet; // 用于存储已抽取的学生名单
random_device rd;
mt19937 gen(rd());
uniform_int_distribution<> dist(0, 0);

// DllMain
BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
    switch (ul_reason_for_call) {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

EXPORT_DLL int DllInit(const wchar_t* filenameW, bool IsAntiRepeat)
{
    //bool isInitialized = false;
    isAntiRepeat = IsAntiRepeat; // 设置是否启用防反复抽取
	students.clear(); // 清空学生名单
	RandomHashSet.clear(); // 清空已抽取的学生名单
    dist = uniform_int_distribution<>(0, 0);
    wstring wstr(filenameW);
    string filename(wstr.begin(), wstr.end());
    ifstream file(filename);
    if (!file) {
        MessageBox(NULL, (L"IslandCaller: Failed to open: " + wstring(filename.begin(), filename.end())).c_str(), L"Error", MB_ICONERROR);
        return -1;
    }
    string name;
    unordered_set<string> ImportHashSet;

    while (getline(file, name)) {
        if (students.size() >= students.max_size()) { // 检查是否达到容器最大容量
            MessageBox(NULL, L"IslandCaller: Student list size exceeds maximum capacity!", L"Error", MB_ICONERROR);
            file.close();
            return -1;
        }
        if( name.empty() || name.find_first_not_of(" \t\n\r") == string::npos) {
            continue; // 跳过空行
		}
        if (ImportHashSet.find(name) != ImportHashSet.end())
        {
            continue; // 跳过重复
        }
        students.push_back(name);
		ImportHashSet.insert(name); // 添加到哈希集以避免重复
    }
    file.close();
    
    // 检查名单是否为空
    if (students.empty()) {
        MessageBox(NULL, L"IslandCaller: Namelist is empty!", L"Error", MB_ICONERROR);
        return -1;
    }

    dist = uniform_int_distribution<>(0, students.size() - 1);
	ImportHashSet.clear(); // 清空导入的哈希集
    isInitialized = true;
    return 0;
}

void ClearHistory()
{
	RandomHashSet.clear(); // 清空已抽取的学生名单
}

//点名器函数
string GetRandomStudent(const int number)
{
    if (!isInitialized)
    {
        return "Not Initialized!";
    }
    string output = "";
    if (number >= students.size())
    {
        return "Not enough students!";// 如果请求的数量超过学生名单，则退出
    }
    // 随机抽取学生
    for (size_t i = 0; i < number; i++)
    {
        if(RandomHashSet.size() >= students.size())
        {
            ClearHistory();
		}
        int randomIndex = dist(gen);
		string randomstu = students[randomIndex];
        if (isAntiRepeat && RandomHashSet.find(randomstu) != RandomHashSet.end())
        {
			i -= 1; // 如果已抽取过该学生，则重新抽取
			continue;
        }
        output += randomstu;
		RandomHashSet.insert(randomstu); // 添加到已抽取名单中
		output += (i == number - 1) ? "" : "  "; // 添加逗号分隔
    }
	return output;
}

//EXPORT wstring GetRandomStudent
EXPORT_DLL BSTR GetRandomStudentName(int number) {
    string name = GetRandomStudent(number);
    wstring_convert<codecvt_utf8_utf16<wchar_t>> converter;
    wstring wstr = converter.from_bytes(name);
    return SysAllocString(wstr.c_str());
}