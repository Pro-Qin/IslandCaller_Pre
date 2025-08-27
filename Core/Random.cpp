#include "pch.h"
using namespace std;

vector<string> students;
bool isInitialized = false;
unordered_set<string> RandomHashSet; // 用于存储已抽取的学生名单

random_device rd; // 随机数生成器
mt19937 gen(rd());
uniform_int_distribution<> dist(0, 0);

EXPORT_DLL int RandomImport(const wchar_t* filenameW)
{
    students.clear(); // 清空学生名单
    RandomHashSet.clear(); // 清空已抽取的学生名单
    dist = uniform_int_distribution<>(0, 0);
    wstring wstr(filenameW);
    string filename(wstr.begin(), wstr.end());
    filename += ".csv";
    char appDataPath[MAX_PATH];
    SHGetFolderPathA(NULL, CSIDL_APPDATA, NULL, 0, appDataPath);
    string filePath = string(appDataPath) + "\\IslandCaller\\Profile\\" + filename;
    ifstream file(filePath);
    if (!file) {
        MessageBox(NULL, (L"IslandCaller: Failed to open: " + wstring(filename.begin(), filename.end())).c_str(), L"Error", MB_ICONERROR);
        return -1;
    }
    string name;
    unordered_set<string> ImportHashSet;
    string line;
    getline(file, line); // 不保存第一行标题
    while (getline(file, line))
    {
        stringstream ss(line);
        string token;
        int columnIndex = 0;
        name.clear();

        while (getline(ss, token, ','))
        {
            if (columnIndex == 1)
            {
                if (!token.empty() && token.front() == '"')
                    token.erase(0, 1);
                if (!token.empty() && token.back() == '"')
                    token.pop_back();
                name = token;
                break;
            }
            columnIndex++;
        }

        if (students.size() >= students.max_size()) {
            MessageBox(NULL, L"IslandCaller: Student list size exceeds maximum capacity!", L"Error", MB_ICONERROR);
            file.close();
            return -1;
        }

        name.erase(0, name.find_first_not_of(" \t\n\r"));
        name.erase(name.find_last_not_of(" \t\n\r") + 1);

        if (name.empty()) continue;
        if (ImportHashSet.find(name) != ImportHashSet.end()) continue;

        students.push_back(name);
        ImportHashSet.insert(name);
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

EXPORT_DLL void ClearHistory()
{
    RandomHashSet.clear(); // 清空已抽取的学生名单
}

//点名器函数
EXPORT_DLL BSTR SimpleRandom(const int number)
{
    std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter; // UTF-8 => UTF-16
    if (!isInitialized)
    {
        return SysAllocString(converter.from_bytes("Not Initialized!").c_str());
    }
    string output = "";
    if (number > students.size())
    {
        return SysAllocString(converter.from_bytes("Not enough students!").c_str());// 如果请求的数量超过学生名单，则退出
    }
    // 随机抽取学生
    for (size_t i = 0; i < number; i++)
    {
        if (RandomHashSet.size() >= students.size())
        {
            ClearHistory();
        }
        int randomIndex = dist(gen);
        string randomstu = students[randomIndex];
        if (RandomHashSet.find(randomstu) != RandomHashSet.end())
        {
            i -= 1; // 如果已抽取过该学生，则重新抽取
            continue;
        }
        output += randomstu;
        RandomHashSet.insert(randomstu); // 添加到已抽取名单中
        output += (i == number - 1) ? "" : "  "; // 添加逗号分隔
    }
    return SysAllocString(converter.from_bytes(output).c_str());
}