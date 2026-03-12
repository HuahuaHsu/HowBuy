# 超級管理員新增其他管理員帳號功能 - 實作完成

## 已完成項目

### ✅ 1. 更新 IAdminService 介面
**位置**: `ISpanShop.Services/Admins/IAdminService.cs`

新增方法：
- `IEnumerable<AdminLevelDto> GetSelectableAdminLevels()` - 取得可選擇的管理員等級（排除超級管理員）
- `(bool IsSuccess, string Message, string GeneratedAccount) CreateAdmin(AdminCreateDto dto)` - 新增管理員
- `(bool IsSuccess, string Message) DeactivateAdmin(int userId, int currentUserId)` - 停用管理員
- `AdminDto? VerifyLogin(string account, string password)` - 驗證管理員登入
- `(bool IsSuccess, string Message) ChangePassword(AdminChangePasswordDto dto)` - 變更管理員密碼

### ✅ 2. 實作 AdminService
**位置**: `ISpanShop.Services/Admins/AdminService.cs`

#### CreateAdmin 邏輯實現
1. ✅ 確認 AdminLevelId 不為 1（不可直接新增超級管理員）
2. ✅ 取得流水號：`int seq = _adminRepository.GetNextAdminSequence()`
3. ✅ 組成帳號與 Email：`string account = $"ADM{seq:D3}"` 及 `string email = $"{account}@ispan.com"`
4. ✅ 確認帳號不重複：`IsAccountExists(account)`
5. ✅ 角色固定為 Admin (roleId = 1)
6. ✅ Hash 初始密碼：`string passwordHash = BCrypt.HashPassword(dto.Password)`
7. ✅ 呼叫 Repository.CreateAdmin()
8. ✅ 回傳 GeneratedAccount 供畫面顯示

#### DeactivateAdmin 邏輯實現
1. ✅ userId != currentUserId（不可停用自己）
2. ✅ 若目標是超級管理員，檢查超級管理員數量 > 1
   - 使用SQL：`SELECT COUNT(1) FROM Users U JOIN AdminLevels AL ON U.AdminLevelId = AL.Id WHERE AL.Id = 1 AND U.IsBlacklisted = 0`
3. ✅ 呼叫 Repository.DeactivateAdmin(userId)

#### VerifyLogin 邏輯實現
1. ✅ 呼叫 GetAdminByAccount(account)
2. ✅ 帳號不存在 → 回傳 null
3. ✅ BCrypt.Verify(password, admin.PasswordHash)
4. ✅ 驗證成功 → 回傳 AdminDto（含 IsFirstLogin）
5. ✅ 驗證失敗 → 回傳 null

#### ChangePassword 邏輯實現
1. ✅ NewPassword == ConfirmPassword（備用檢查，主要由 DataAnnotation 驗證）
2. ✅ 新密碼至少 8 碼且含英文與數字（使用 Regex 驗證）
3. ✅ Hash 新密碼：`string newPasswordHash = BCrypt.HashPassword(dto.NewPassword)`
4. ✅ 呼叫 Repository.ChangePassword()
5. ✅ 呼叫 Repository.SetFirstLoginComplete()

### ✅ 3. 更新 IAdminRepository 介面
**位置**: `ISpanShop.Repositories/Admins/IAdminRepository.cs`

新增方法簽名：
- `IEnumerable<AdminLevelDto> GetSelectableAdminLevels()`
- `AdminDto? GetAdminByAccount(string account)`
- `int GetNextAdminSequence()`
- `bool CreateAdmin(string account, string email, string passwordHash, int roleId, int adminLevelId)`
- `bool DeactivateAdmin(int userId)`
- `bool IsAccountExists(string account)`
- `bool ChangePassword(int userId, string newPasswordHash)`
- `bool SetFirstLoginComplete(int userId)`
- `int GetSuperAdminCount()`

### ✅ 4. 在 AdminRepository 新增缺失的方法
**位置**: `ISpanShop.Repositories/Admins/AdminRepository.cs`

新增方法實現：
- `GetSuperAdminCount()` - 計算有效的超級管理員數量

## 技術細節

### 密碼安全性
- 使用 BCrypt.Net-Next (v4.1.0) 進行密碼 Hash 和驗證
- 初始密碼由系統生成並由超級管理員設定
- 密碼變更時進行強度驗證（至少8碼、含英文字母、含數字）

### 帳號生成
- 帳號格式：`ADM{序列號:D3}` (例如: ADM001, ADM002, ADM999)
- 郵箱格式：`{帳號}@ispan.com`
- 自動檢查帳號重複性

### 管理員等級保護
- 無法直接新增超級管理員（AdminLevelId = 1）
- 停用時若為超級管理員，需確保至少保留一位超級管理員

### 自我保護機制
- 無法停用自己的帳號
- 無法修改自己的角色（現有功能）

## 使用的 DTOs
- `AdminDto` - 管理員基本資訊（已包含 PasswordHash、IsFirstLogin 等）
- `AdminLevelDto` - 管理員等級選項
- `AdminCreateDto` - 新增管理員輸入（Password、AdminLevelId）
- `AdminChangePasswordDto` - 變更密碼輸入（NewPassword、ConfirmPassword）
- `AdminPermissionDto` - 角色權限選項（現有功能）

## 備註
- 所有方法均包含異常處理，返回用戶友善的錯誤訊息
- Repository 層已實現所有所需的資料庫操作
- 準備好整合到 Controller 層進行 API 端點開發
