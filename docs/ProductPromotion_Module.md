# 商品與活動/促銷模組說明

更新日期：2026-05-09

## 1. 模組範圍

商品模組涵蓋前台商品瀏覽、商品詳情、賣家商品管理、商品規格與圖片管理，以及後台商品審核。促銷模組涵蓋前台活動展示、活動商品列表、商品活動標籤、賣家活動建立與管理、活動商品綁定，以及後台活動審核。

本文件聚焦兩條主線：

- 商品：買家看商品、賣家建商品、後台審商品。
- 活動/促銷：買家看活動、賣家建活動並綁商品、後台審活動。

相關但非主軸的功能包含分類、品牌、購物車、訂單折抵與優惠券。這些功能會被商品/活動流程引用，但不是本文件的主要展開範圍。

## 2. 前端頁面與路由

前端使用 Vue 3、Vite、TypeScript、Vue Router、Pinia 與 Element Plus。主要路由定義在 `ISpanShop-Frontend/src/router/routes.ts`。

| 使用情境 | 路由 | 頁面 |
|---|---|---|
| 前台商品列表 | `/products` | `src/views/ProductsView.vue` |
| 前台商品詳情 | `/product/:id` | `src/views/ProductDetailView.vue` |
| 前台活動詳情 | `/promotion/:id` | `src/views/PromotionView.vue` |
| 賣家商品列表 | `/seller/products` | `src/views/seller/ProductListView.vue` |
| 賣家新增商品 | `/seller/products/new` | `src/views/seller/ProductEditView.vue` |
| 賣家編輯商品 | `/seller/products/:id/edit` | `src/views/seller/ProductEditView.vue` |
| 賣家商品預覽 | `/seller/products/:id/preview` | `src/views/seller/ProductPreviewView.vue` |
| 賣家活動管理 | `/seller/promotions` | `src/views/seller/PromotionListView.vue` |

前台頁面使用 `DefaultLayout`，商品與活動公開頁不需要登入。賣家中心使用 `SellerLayout`，`/seller` 底下路由標記 `requiresAuth: true` 與 `requiresSeller: true`。

前端 API 封裝：

- 商品：`ISpanShop-Frontend/src/api/product.ts`
- 活動：`ISpanShop-Frontend/src/api/promotion.ts`
- Axios request 攔截器：`ISpanShop-Frontend/src/api/request.ts`

## 3. 後端 API Controller

後端同時提供前台 REST API、賣家中心 API 與後台 MVC/Razor 管理頁。

### 前台商品 API

Controller：`ISpanShop.MVC/Controllers/Api/Products/ProductsApiController.cs`

| Method | Endpoint | 說明 | 權限 |
|---|---|---|---|
| GET | `/api/products` | 商品列表，支援分類、品牌、價格、關鍵字、排序、分頁 | AllowAnonymous |
| GET | `/api/products/{id}` | 商品詳情，含圖片、規格、分類路徑、店家資訊、評分摘要 | AllowAnonymous |
| GET | `/api/products/{id}/related` | 同分類相關商品 | AllowAnonymous |
| GET | `/api/hot-keywords` | 熱搜關鍵字 | AllowAnonymous |

### 賣家商品 API

Controller：`ISpanShop.MVC/Controllers/Api/Products/SellerProductsApiController.cs`

| Method | Endpoint | 說明 | 權限 |
|---|---|---|---|
| GET | `/api/seller/products` | 賣家商品列表，從 JWT StoreId 過濾 | FrontendJwt |
| GET | `/api/seller/products/{id}` | 賣家商品詳情，驗證商品屬於該賣家 | FrontendJwt |
| POST | `/api/seller/products` | 新增商品，multipart/form-data，支援圖片 | FrontendJwt |
| PUT | `/api/seller/products/{id}` | 編輯商品，審核中商品不可編輯 | FrontendJwt |
| DELETE | `/api/seller/products/{id}` | 軟刪除商品 | FrontendJwt |
| PUT | `/api/seller/products/{id}/images` | 更新商品圖片 | FrontendJwt |
| PATCH | `/api/seller/products/{id}/status` | 商品上架/下架 | FrontendJwt |
| PUT | `/api/seller/products/{id}/submit-review` | 草稿送出審核 | FrontendJwt |
| POST | `/api/seller/products/upload-image` | 描述編輯器圖片上傳 | FrontendJwt |

規格 API：`ISpanShop.MVC/Controllers/Api/Products/SellerVariantsApiController.cs`

| Method | Endpoint | 說明 |
|---|---|---|
| GET | `/api/seller/products/{productId}/variants` | 取得商品規格 |
| POST | `/api/seller/products/{productId}/variants` | 新增規格 |
| PUT | `/api/seller/products/{productId}/variants/{variantId}` | 更新規格 |
| DELETE | `/api/seller/products/{productId}/variants/{variantId}` | 軟刪除規格 |

### 前台活動 API

Controller：`ISpanShop.MVC/Controllers/Api/Promotions/PromotionApiController.cs`

| Method | Endpoint | 說明 | 權限 |
|---|---|---|---|
| GET | `/api/promotions/active` | 目前進行中的活動，可依類型篩選 | AllowAnonymous |
| GET | `/api/promotions/{id}` | 活動詳情 | AllowAnonymous |
| GET | `/api/promotions/{id}/products` | 活動商品列表，支援分頁與排序 | AllowAnonymous |
| GET | `/api/promotions/product/{productId}` | 指定商品目前參與的活動 | AllowAnonymous |

### 賣家活動 API

Controller：`ISpanShop.MVC/Controllers/Api/Promotions/SellerPromotionsApiController.cs`

| Method | Endpoint | 說明 | 權限 |
|---|---|---|---|
| GET | `/api/seller/promotions` | 賣家活動列表，支援狀態與分頁 | FrontendJwt |
| POST | `/api/seller/promotions` | 建立活動，預設待審核 | FrontendJwt |
| GET | `/api/seller/promotions/{id}` | 單一活動詳情，驗證賣家所有權 | FrontendJwt |
| PUT | `/api/seller/promotions/{id}` | 編輯活動，依狀態限制可編輯欄位 | FrontendJwt |
| DELETE | `/api/seller/promotions/{id}` | 軟刪除活動 | FrontendJwt |
| DELETE | `/api/seller/promotions/{id}/cancel-review` | 撤銷待審核活動 | FrontendJwt |
| PUT | `/api/seller/promotions/{id}/end-early` | 提早結束進行中活動 | FrontendJwt |

活動商品綁定 API：`ISpanShop.MVC/Controllers/Api/Promotions/SellerPromotionItemsApiController.cs`

| Method | Endpoint | 說明 |
|---|---|---|
| GET | `/api/seller/promotions/{promotionId}/products` | 取得已綁定商品 |
| POST | `/api/seller/promotions/{promotionId}/products` | 批次綁定商品 |
| DELETE | `/api/seller/promotions/{promotionId}/products/{productId}` | 移除活動商品 |
| GET | `/api/seller/promotions/{promotionId}/available-products` | 可加入活動的商品，支援搜尋與分頁 |
| POST | `/api/seller/promotions/{promotionId}/fix-prices` | 修復活動商品價格資料 |

### 後台審核入口

商品審核：`ISpanShop.MVC/Areas/Admin/Controllers/Products/ProductsController.cs`

- 待審核、退回、重新送審、近期通過列表。
- 核准、退回、批次核准、批次退回。
- 強制下架、重新申請審核、退回商品到期清理。
- Demo 用敏感字自動審核與測試商品產生。

活動審核：`ISpanShop.MVC/Controllers/Promotions/PromotionsController.cs`

- 後台活動列表與詳情。
- 活動核准、拒絕、模擬賣家重新送審、刪除。

## 4. Service / Repository / DTO / Entity 對應

### 商品

| 層級 | 檔案/型別 | 職責 |
|---|---|---|
| Controller | `ProductsApiController` | 前台商品公開查詢 |
| Controller | `SellerProductsApiController` | 賣家商品 CRUD、圖片、狀態、送審 |
| Controller | `SellerVariantsApiController` | 商品規格 CRUD |
| Service | `IProductService`, `ProductService` | 商品建立、更新、審核、分頁查詢、詳情組裝 |
| Repository | `IProductRepository`, `ProductRepository` | EF Core 查詢、Skip/Take 分頁、狀態篩選、審核資料操作 |
| DTO | `ProductSearchCriteria` | 商品列表查詢條件 |
| DTO | `ProductListDto`, `ProductDetailDto` | Service 回傳資料 |
| DTO | `ProductApiDtos.cs` | 前台/賣家 API request/response |
| DTO | `ProductPromotionInfoDto` | 商品列表與詳情使用的活動資訊 |
| Entity | `Product`, `ProductVariant`, `ProductImage` | 商品主檔、規格、圖片 |

商品主要 Entity 關聯：

- `Product.StoreId` 對應店家。
- `Product.CategoryId` 對應分類。
- `Product.BrandId` 可選品牌。
- `Product.ProductVariants` 保存價格、庫存與規格值 JSON。
- `Product.ProductImages` 保存主圖與排序。
- `Product.PromotionItems` 連到活動商品。

### 活動/促銷

| 層級 | 檔案/型別 | 職責 |
|---|---|---|
| Controller | `PromotionApiController` | 前台活動公開查詢 |
| Controller | `SellerPromotionsApiController` | 賣家活動 CRUD、狀態操作 |
| Controller | `SellerPromotionItemsApiController` | 活動商品綁定與可加入商品查詢 |
| Service | `PromotionService` | 活動類型/狀態轉換、活動查詢、建立、更新、刪除 |
| Repository | `IPromotionRepository`, `PromotionRepository` | 活動分頁、進行中活動查詢、軟刪除 |
| DTO | `CreatePromotionDto`, `UpdatePromotionDto` | 賣家建立/編輯活動 |
| DTO | `SellerPromotionListDto` | 賣家活動列表與詳情 |
| Entity | `Promotion`, `PromotionItem`, `PromotionRule` | 活動主檔、活動商品、活動規則 |

活動主要 Entity 關聯：

- `Promotion.SellerId` 對應建立活動的賣家 User。
- `Promotion.PromotionType`：1=限時特賣、2=滿額折扣、3=限量搶購。
- `Promotion.Status`：0=待審核、1=核准、2=拒絕、3=已結束/提早結束。
- `PromotionRule` 保存門檻、折扣類型與折扣值。
- `PromotionItem` 保存活動商品、原價、活動價、折扣百分比、銷售數與限制數量。

目前活動商品綁定 Controller 直接使用 `ISpanShopDBContext`，而不是經過 Repository。這讓功能開發速度快，但未來若要強化可測試性與一致性，可以把綁定/移除/可加入商品查詢抽到 Service 或 Repository。

## 5. 主要使用者流程

### 買家商品瀏覽流程

1. 使用者進入 `/products`。
2. 前端讀取 query string：`keyword`、`categoryId`、`subCategoryId`、`brandIds`、`minPrice`、`maxPrice`、`sortBy`、`page`。
3. `ProductsView.vue` 呼叫 `fetchProductList()`。
4. 後端 `/api/products` 呼叫 `GetFrontActiveProductsAsync()`。
5. Repository 只查詢已上架、未刪除、賣場未停權、賣家未黑名單商品。
6. Controller 批次補上商品正在參加的活動資訊。
7. 前端渲染商品卡片、分頁、排序與篩選狀態。

### 買家商品詳情流程

1. 使用者進入 `/product/:id`。
2. `ProductDetailView.vue` 呼叫 `fetchProductDetail(id)`。
3. 後端取得商品、圖片、規格、分類路徑、品牌、店家資訊。
4. 後端 fire-and-forget 累加瀏覽次數。
5. 前端解析規格軸，讓買家選擇 variant。
6. 前端另外呼叫 `fetchProductPromotions(id)` 顯示商品參與中的活動。
7. 前端呼叫 `fetchRelatedProducts(id)` 顯示相關商品。

### 買家活動瀏覽流程

1. 首頁或其他入口取得 `/api/promotions/active`。
2. 使用者進入 `/promotion/:id`。
3. `PromotionView.vue` 呼叫 `fetchPromotionById(id)` 取得活動資訊。
4. 同頁呼叫 `fetchPublicPromotionProducts(id, { page, pageSize, sortBy, priceOrder })`。
5. 後端只回傳有效活動、已上架且未刪除的活動商品。
6. 前端支援活動商品排序、價格排序與分頁。

## 6. 商品審核流程

商品審核同時使用 `Status` 與 `ReviewStatus`。

常見狀態語意：

| 欄位 | 值 | 意義 |
|---|---:|---|
| `Product.Status` | 0 | 未上架/下架 |
| `Product.Status` | 1 | 已上架 |
| `Product.Status` | 2 | 待審核 |
| `Product.Status` | 3 | 審核退回 |
| `Product.Status` | 4 | 強制下架 |
| `Product.ReviewStatus` | 0 | 待審核 |
| `Product.ReviewStatus` | 1 | 審核通過 |
| `Product.ReviewStatus` | 2 | 退回 |
| `Product.ReviewStatus` | 3 | 重新送審 |
| `Product.ReviewStatus` | 4 | 草稿 |

流程：

1. 賣家新增商品時選擇草稿或送審。
2. 草稿：`Status=0`、`ReviewStatus=4`。
3. 送審：`Status=2`、`ReviewStatus=0`。
4. 後台商品審核中心讀取待審核清單。
5. 管理員核准：商品改為上架，`Status=1`、`ReviewStatus=1`。
6. 管理員退回：商品改為退回，記錄 `RejectReason`。
7. 賣家修改後重新送審，進入重新申請審核流程。
8. 後台可對重新送審商品核准或駁回。

後台商品審核也包含 Demo 輔助能力，例如批次產生測試商品、敏感字自動審核、退回商品倒數清理。這些功能集中在 Admin 商品 Controller 與 Razor View，不影響前台公開商品查詢。

## 7. 賣家商品管理流程

賣家商品管理頁是 `/seller/products`。

主要能力：

- 商品列表：依 Tab 顯示全部、已上架、已下架、審核中、已退回、草稿、已刪除。
- 搜尋與篩選：關鍵字、分類、價格區間。
- 排序：建立時間、價格、庫存、銷量等。
- 分頁：前端傳 `page` 與 `pageSize=20`，後端 Skip/Take 回傳當頁資料。
- 新增商品：可儲存草稿或直接送審。
- 編輯商品：審核中不可編輯，退回商品可修改後重新送審。
- 上下架：只允許已通過審核的商品在上架/下架間切換。
- 刪除：使用軟刪除。
- 圖片：商品主圖與多圖上傳到 `wwwroot/uploads/products`。
- 規格：以 variant 保存 SKU、價格、庫存、規格 JSON。

賣家商品 API 不信任前端傳入的 StoreId，而是從 JWT claim 取 StoreId，避免賣家偽造別人的店家 ID。

## 8. 活動建立與活動商品關聯流程

賣家活動管理頁是 `/seller/promotions`，同一頁內包含列表、建立/編輯表單、活動商品選擇與詳情檢視。

活動類型：

| PromotionType | 類型 | 規則 |
|---:|---|---|
| 1 | 限時特賣 | 百分比折扣 |
| 2 | 滿額折扣 | 門檻金額 + 折抵金額 |
| 3 | 限量搶購 | 單品折抵金額 + 限量資訊 |

建立流程：

1. 賣家填寫活動名稱、描述、類型、開始/結束時間與折扣值。
2. 前端呼叫 `POST /api/seller/promotions`。
3. 後端建立 `Promotion`，預設 `Status=0` 待審核。
4. 後端依活動類型建立 `PromotionRule`。
5. 賣家在活動中選擇可加入商品。
6. 前端呼叫 `POST /api/seller/promotions/{promotionId}/products` 批次綁定。
7. 後端驗證商品屬於該賣家 StoreId，排除已綁定商品，並依規則計算活動價。
8. 管理員在後台核准後，活動 `Status=1`。
9. 前台只顯示已核准、時間區間內、未刪除、賣家未黑名單、店家未停權的活動。

編輯與刪除限制：

- 待審核活動可撤銷送審。
- 已拒絕活動可完整編輯後重新送審。
- 即將開始活動只允許更新描述，避免改變買家預期。
- 進行中活動不可直接刪除，需先提早結束。
- 即將開始活動不允許移除商品，以保障已加購物車的買家權益。

## 9. 分頁、搜尋、排序、狀態篩選設計

### 前台商品列表

Endpoint：`GET /api/products`

參數：

- `categoryId`：主分類，後端會展開直接子分類。
- `subCategoryId`：子分類，優先於主分類。
- `brandIds`：品牌多選。
- `minPrice`、`maxPrice`：價格區間，以 `MinPrice` 比較。
- `keyword`：商品名稱搜尋。
- `sortBy`：`latest`、`priceAsc`、`priceDesc`、`soldCount`。
- `page`、`pageSize`：預設 1 / 20，後端限制 pageSize 上限 50。

Repository 使用 `CountAsync()` 取得總筆數，再用 `Skip((page - 1) * pageSize).Take(pageSize)` 取得當頁資料。

### 賣家商品列表

Endpoint：`GET /api/seller/products`

參數：

- `keyword`
- `categoryId`
- `parentCatId`
- `brandId`
- `status`
- `tab`
- `sortBy`
- `page`
- `pageSize`

賣家 Tab 對應：

| tab | 條件 |
|---|---|
| `on` | `Status=1` 且未刪除 |
| `off` | `Status=0`、`ReviewStatus=1` 且未刪除 |
| `review` | `Status=2` 且未刪除 |
| `rejected` | `Status=3` 且未刪除 |
| `draft` | `Status=0`、`ReviewStatus!=1` 且未刪除 |
| `deleted` | `IsDeleted=true` |
| `all` | 不帶 tab，查詢賣家全部商品 |

排序包含 `date_desc`、`date_asc`、`price_asc`、`price_desc`、`stock_desc`、`stock_asc`、`sales_desc`、`sales_asc` 等。

### 前台活動商品列表

Endpoint：`GET /api/promotions/{id}/products`

參數：

- `page`
- `pageSize`
- `sortBy`
- `priceOrder`

排序：

- `sales`：活動商品售出數高到低。
- `priceAsc`/`priceDesc` 或 `priceOrder=asc/desc`：依活動價或原價排序。
- 預設依 `PromotionItem.Id`。

### 賣家活動列表

Endpoint：`GET /api/seller/promotions`

參數：

- `status`：`all`、`pending`、`active`、`upcoming`、`rejected`、`ended`。
- `page`
- `pageSize`

Repository 先依 `SellerId` 與 `IsDeleted=false` 過濾，再依狀態篩選，最後依 `CreatedAt` 倒序並使用 Skip/Take 分頁。

## 10. 權限與驗證方式

本專案有前台 JWT 與後台 Cookie 兩套驗證。

前台/賣家中心：

- 公開商品與公開活動 API 使用 `[AllowAnonymous]`。
- 賣家商品、賣家活動、活動商品綁定 API 使用 `[Authorize(AuthenticationSchemes = "FrontendJwt")]`。
- Axios request interceptor 自動從 `localStorage.token` 帶 `Authorization: Bearer ...`。
- Vue Router 對 `/seller` 使用 `requiresAuth` 與 `requiresSeller`。
- 賣家商品 API 從 JWT `StoreId` 取得店家身份。
- 賣家活動 API 從 JWT `ClaimTypes.NameIdentifier` 取得賣家 UserId。
- 黑名單使用者會被前端路由守衛限制；公開商品/活動查詢也會排除黑名單賣家與停權店家。

後台：

- Admin Area 使用 Cookie Authentication。
- 商品審核與後台管理介面在 `Areas/Admin`。
- 活動後台 Controller 也以後台頁面流程操作審核。

重要安全點：

- 賣家不能透過前端傳入 StoreId 操作別人的商品。
- 編輯、刪除、活動操作都會檢查資料是否屬於目前賣家。
- 前台公開查詢排除已刪除、未上架、停權店家與黑名單賣家資料。

## 11. 可用於簡報的技術亮點

- 前後台分離：同一套 ASP.NET Core host 同時提供 Admin MVC/Razor 與 Vue SPA REST API。
- 分層架構：Controller、Service、Repository、DTO、EF Entity 職責清楚，商品模組完整走分層。
- 後端分頁：商品與活動列表都使用後端 Skip/Take 與總筆數回傳，避免前端全量載入。
- 商品規格彈性：variant 使用 `SpecValueJson` 保存多規格組合，可支援不同分類的規格軸。
- 商品審核狀態完整：支援草稿、送審、通過、退回、重新送審、強制下架與軟刪除。
- 促銷規則抽象：`Promotion`、`PromotionRule`、`PromotionItem` 拆開，讓不同活動類型可共用活動商品模型。
- 活動價計算：綁定活動商品時保存原價、活動價、折扣百分比，前台顯示時再依有庫存最低價校正。
- 權限防偽：賣家操作從 JWT 取 UserId/StoreId，不信任前端傳入身份。
- 前台可用性：商品列表支援 query string 狀態，搜尋、分類、品牌、價格、排序與分頁可被分享或返回。
- 後台 Demo 支援：商品審核中心有批次審核、敏感字自動審核、退回倒數清理等展示流程。

## 12. 面試可能被問到的問題與回答方向

### Q1：商品列表為什麼要做後端分頁？

因為商品資料量會持續成長，前端全量載入再切片會造成 API 傳輸量大、首次渲染慢，也讓搜尋排序結果不準。後端分頁讓資料庫直接用 `COUNT + Skip/Take` 回傳當頁資料，前端只保存目前頁，擴充性比較好。

### Q2：商品的 Status 和 ReviewStatus 為什麼分開？

`Status` 偏向商品是否可被前台販售，例如上架、下架、待審核、退回；`ReviewStatus` 偏向審核流程，例如待審核、通過、退回、重新送審、草稿。兩者分開可以同時描述販售狀態與審核狀態，但也需要在 Service/Controller 維持狀態轉換一致性。

### Q3：如何避免賣家操作別人的商品？

賣家 API 不使用前端傳入的 StoreId 作為信任來源，而是從 JWT claim 取 StoreId 或 UserId。查詢、編輯、刪除時都會檢查資料是否屬於目前賣家，不符合就回 403。

### Q4：商品多規格怎麼設計？

商品主檔保存共通資訊，`ProductVariant` 保存每個規格組合的價格、庫存、SKU 與 `SpecValueJson`。例如同一商品可以有顏色、尺寸等不同組合；詳情頁會把 variant 的 JSON 彙整成可選規格軸。

### Q5：活動模組如何支援不同促銷類型？

活動主檔 `Promotion` 保存名稱、時間、狀態與類型；`PromotionRule` 保存折扣規則；`PromotionItem` 保存活動與商品的關聯及活動價。不同類型透過 `PromotionType` 和 rule 的 `DiscountType`、`DiscountValue`、`Threshold` 表達。

### Q6：促銷活動為什麼也需要審核？

活動會影響前台價格與買家期待，需要避免不合理折扣、違規文字或錯誤活動上線。賣家建立後先進入待審核，後台核准後才會在前台公開顯示。

### Q7：前台活動商品為什麼要再檢查商品狀態？

即使活動本身有效，活動中的商品也可能被下架、刪除或店家停權。前台查詢必須再次過濾商品 `Status=1` 與 `IsDeleted=false`，避免使用者點進不存在或不可購買的商品。

### Q8：目前架構還可以怎麼改善？

商品模組分層較完整；活動商品綁定目前在 Controller 直接使用 `DbContext`，未來可以抽到 Service/Repository，讓驗證、價格計算、綁定邏輯更容易測試與重用。另外，活動列表若需要更複雜排序或統計卡片，也可以補專用 counts API，避免前端多次查詢。

### Q9：如果活動商品價格和商品 variant 價格不同步怎麼辦？

目前綁定活動商品時會保存當下原價與活動價；前台顯示活動商品時會再依有庫存 variant 的最低價計算顯示價格。這能降低舊資料造成的顯示錯誤，但長期更理想的做法是明確定義價格快照與即時計價的優先順序。

### Q10：如何說明這個模組的核心價值？

這個模組不是單純 CRUD，而是把電商常見的商品生命週期、賣家權限、後台審核、促銷規則、活動商品關聯與前台分頁搜尋串在一起。它展示了從資料模型、API 設計、權限驗證到前端互動的一條完整業務流程。
