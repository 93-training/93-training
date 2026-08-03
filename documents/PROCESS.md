# PROCESS.md — 我的練習心得

> 一個原則：**寫「具體發生的事」，不寫感想文。**
> 貼上當時真實的 prompt、真實的數字、真實的錯誤訊息——三個月後的你（和你的同事）才用得上。

#### 使用的 agent 與模型：

---
Claude CLI

## 通用四問

### 1. 我的任務拆解

（開工前你把任務拆成哪幾步？實際做的時候順序有變嗎？為什麼變？）

-

### 2. AI 幫上大忙的地方

（哪件事 agent 做得又快又好？**貼上當時的提問原文**，說明為什麼這樣問有效。）

-

### 3. AI 誤導我的地方，與我如何發現

（agent 說錯／改錯／過度自信的時刻。你靠什麼抓到——對照程式碼？頁面實測？跑測試？）

-

### 4. 我會帶回日常工作的一招

（一個具體、可複製的做法，不要寫「要多驗證」這種口號——寫出**操作步驟**。）

-

## 自我驗證（做到哪個階段答哪題）

### 第一階段 — Agentic Coding

練習 1

1. 我能不看筆記說出三個專案（Web/Core/Infrastructure）各自的職責 (v)
2. 我核對過 agent 描述的建單流程，且**至少找出一處不精確或過度簡化的說法** (v)
3. 我知道商業邏輯應該放在哪一層、新增頁面要動哪些地方 (v)

練習 2

1. 三個 bug 我都先在頁面上重現過，才開始找程式 (v)
   - 對比活動2練習0， AI agent使用playwright來測試建立訂單，之後給出測試步驟和結果及說明，當然執行的每一步驟都有詢問是否允許執行
   測試步驟

┌─────┬────────────────────────────────────────┬───────────────────────────────────────────────────────────────────────────┐
│  #  │                  動作                  │                                   結果                                    │
├─────┼────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────────┤
│ 1   │ 開啟 /Orders/Create                    │ 客戶（20 筆）與商品（47 筆）下拉正常載入                                  │
├─────┼────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────────┤
│ 2   │ 選客戶「陳志明（金卡會員）」           │ ✔                                                                         │
├─────┼────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────────┤
│ 3   │ 第 1 列：SKU-1002 極光 機械鍵盤 × 3    │ ✔                                                                         │
├─────┼────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────────┤
│ 4   │ 點「新增一列」（JS 重新編號 Lines[i]） │ ✔ 第二列 Lines_1__* 正確產生                                              │
├─────┼────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────────┤
│ 5   │ 第 2 列：SKU-1007 極光 降噪耳機 × 2    │ ✔                                                                         │
├─────┼────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────────┤
│ 6   │ 送出                                   │ 302 → /Orders/Details/207，TempData["Success"] 顯示「訂單 #207 建立成功」 │
└─────┴────────────────────────────────────────┴───────────────────────────────────────────────────────────────────────────┘

** 驗證結果

┌─────────────────┬──────────────────────────┬────────────────────────┐
│      項目       │            值            │          檢查          │
├─────────────────┼──────────────────────────┼────────────────────────┤
│ SKU-1002 小計   │ 3 × 2,320 = 6,960        │ ✔                      │
├─────────────────┼──────────────────────────┼────────────────────────┤
│ SKU-1007 小計   │ 2 × 1,930 = 3,860        │ ✔                      │
├─────────────────┼──────────────────────────┼────────────────────────┤
│ 小計            │ 10,820                   │ ✔                      │
├─────────────────┼──────────────────────────┼────────────────────────┤
│ 會員折扣（10%） │ -1,082                   │ 金卡套用 10%           │
├─────────────────┼──────────────────────────┼────────────────────────┤
│ 應付總額        │ 9,738                    │ ✔ 與 10,820 × 0.9 相符 │
├─────────────────┼──────────────────────────┼────────────────────────┤
│ 狀態            │ 待處理                   │ ✔                      │
├─────────────────┼──────────────────────────┼────
│ 單價快照        │ 有寫入 UnitPriceSnapshot │ ✔                      │
└─────────────────┴──────────────────────────┴────

金卡折扣率跑出來是 10%——這正好是 CLAUDE.md 點名的math）。我沒有改動任何程式碼，只是回報實際行為；要不要動它由你決定。

** 幾點說明

- 我原本要啟動 app，但 port 5150 已經有一個 instance 在跑（dotnet run 回報 address already in use），所以是對既有的那個做測試。dotnet build 是乾
- 這筆 #207 是真的寫進資料庫了，庫存也已扣（SKU-1002 102→99、SKU-1007 52→50）。要清掉的話可以在畫面上按「取消訂單」，或 dotnet ef database drop -f -p src/OrderHub.Infrastructure -s
- 截圖已移到 scratchpad，沒有留在 repo 裡。但 Playwright 在 repo 根目錄留下了 .playwright-mcp/（6 個 .yml snapshot 檔），我要刪的指令被拒絕了 —— 需要的話請自行 rm -rtignore。
  
2. 我給 agent 的資訊包含具體觀察（頁碼／金額數字／庫存數字），而不是只貼客訴原文 (v)
3. 每個修復都回到頁面驗證過症狀消失 (v)
4. 每個 bug 都補了一個回歸測試，`dotnet test` 全綠 (v)
5. 三個獨立 commit，message 說明症狀與根因 (v)
6. （思考題）為什麼原本的測試沒抓到這三個 bug？(v) 因為原本的測試也是在錯誤的logic上跑測試

練習 3

1. `/Products/LowStock` 不帶參數 → 門檻 10 的結果；帶 `?threshold=3` → 結果隨之改變  (v)
2. `?threshold=0`、`?threshold=-1` → 頁面顯示驗證錯誤，不是 500 (v， 有顯示錯誤信息)
3. 售出數量欄位排除了 Cancelled 訂單（可用一筆已取消的訂單驗證） (v)
4. 停售（已停售 badge）商品不出現在列表  (v)
5. 程式分層與命名跟既有的 Products 功能一致（請 agent 自我 review 一次，並自己確認） (v)
6. 至少 3 個新測試，`dotnet test` 全綠  (v)

練習 4

1. 重構後 `dotnet test` 全綠  (v)
2. 我能說出這次重構「改善了什麼、沒有改變什麼」  (v)
3. 我有在 code review 的角度看過 diff（不是 agent 說好就好）  (v)

---

## 附錄：值得留下的對話片段

（貼 1–2 段最有代表性的 prompt 與回應**摘要**——不用貼全文，重點是「我怎麼問」和「它怎麼答」。）

### 練習 4 - prompt
 i found that @OrderService.CreateOrderAsync has dynamically increasing its validation logic as validation requirements growing, please help to revamp/modify the structure to make it simple and precise, following instructions below
  1. all behaviours created before / existed remains unchanged, including previously commited
  2. plan first and explain your re-vamp details
  3. do an independent commit for this

### Claude 回答
  Here's my proposed revamp of OrderService.CreateOrderAsync.

The problem: All validation lives inline in one ~55-line method — four sequential header guards plus a per-line loop that mixes validation and mutation. Each new rule just gets appended, so it keeps growing.

The revamp (only OrderService.cs changes, public signature untouched):
- Public method becomes a short linear orchestration: fetch customer → validate lines → build order → build items → save.
- ValidateLines(...) — one focused home for line-shape rules (empty / non-positive qty / duplicates), first-fail-wins. Future rules land here, not in the public method.
- BuildOrderItemsAsync(...) — the per-line inventory processing/error-accumulation, moved out verbatim.

Behavior is preserved exactly — same zh-TW messages, same check order, same error-accumulation semantics, same in-loop stock mutation timing, no persistence on failure. No test edits; the 11 OrderServiceCreateTests are the contract. I deliberately avoided a rule-engine/pipeline abstraction as over-engineering for this small internal app (per CLAUDE.md). Then a single independent commit containing only that file.

Revamp OrderService.CreateOrderAsync validation structure

Context

OrderService.CreateOrderAsync (src/OrderHub.Core/Services/OrderService.cs:35-92) has
accreted validation over time: four sequential up-front guard clauses (customer,
empty lines, non-positive quantity, duplicate product) followed by a per-line loop that
both validates (existence/active, stock) and mutates (decrements stock, snapshots price,
builds items). Everything lives in one ~55-line method, so each new rule gets appended
inline and the method keeps growing. The goal is to make the structure simple and
precise — a short, readable orchestration with each concern in its own focused place —
without changing any existing behavior.

Constraint: behavior must stay identical

All observable behavior is pinned by tests/OrderHub.Tests/OrderServiceCreateTests.cs
(11 tests). The refactor preserves, exactly:

- Same messages (verbatim zh-TW strings).
- Same check order (first-fail-wins for header rules): customer → empty → quantity → duplicate.
- Same per-line semantics: every line is processed, errors are accumulated into a list,
and on any error the method returns Fail(errors) with no AddAsync/SaveChangesAsync
(so nothing is persisted — CreateOrder_Failed_DoesNotPersistOrder).
- Same stock-mutation timing: product.StockQuantity -= line.Quantity stays inside the
per-line loop (in-memory, unsaved on failure) — not hoisted to a validate-then-apply pass,
which would be a behavior change in the tracked-context edge case.
- Public signature unchanged — IOrderService.CreateOrderAsync is untouched; no controller,
interface, or test edits needed.

Approach — extract two private helpers, keep the public method as a thin orchestrator

Only src/OrderHub.Core/Services/OrderService.cs changes. Refactor the body of
CreateOrderAsync and add two private methods; nothing else in the class moves.

1. CreateOrderAsync becomes a short linear read:

public async Task<ServiceResult<Order>> CreateOrderAsync(int customerId, IReadOnlyList<NewOrderLine> lines)
{
    var customer = await _customerRepository.GetByIdAsync(customerId);
    if (customer is null)
        return ServiceResult<Order>.Fail("找不到指定的客戶");

    var lineError = ValidateLines(lines);
    if (lineError is not null)
        return ServiceResult<Order>.Fail(lineError);

    var order = new Order
    {
        CustomerId = customer.Id,
        Status = OrderStatus.Pending,
        CreatedAt = DateTime.UtcNow
    };

    var errors = await BuildOrderItemsAsync(ord
    if (errors.Count > 0)
        return ServiceResult<Order>.Fail(errors

    await _orderRepository.AddAsync(order);
    await _orderRepository.SaveChangesAsync();

    return ServiceResult<Order>.Ok(order);
}

The customer null-check stays inline because it used
downstream (it isn't a pure line-shape rule).

2. ValidateLines — the single home for request-shape (line) rules. This is where the
"validation keeps growing" pressure is now absocome one more
guard clause here, in order, instead of bloating the public method. First failure wins,
returns the message or null:

private static string? ValidateLines(IReadOnlyL
{
    if (lines is null || lines.Count == 0)
        return "訂單至少需要一項商品";
    if (lines.Any(l => l.Quantity <= 0))
        return "商品數量必須大於 0";
    if (lines.Select(l => l.ProductId).Distinct
        return "同一商品請勿重複加入，請調整數量即可";
    return null;
}

3. BuildOrderItemsAsync — per-line inventory processing + item building, accumulating
errors (unchanged logic, just moved out of the

private async Task<List<string>> BuildOrderItemList<NewOrderLine> lines)
{
    var errors = new List<string>();
    foreach (var line in lines)
    {
        var product = await _productRepository.GetByIdAsync(line.ProductId);
        if (product is null || !product.IsActiv
        {
            errors.Add('$"商品（Id={line.Product
            continue;
        }
        if (product.StockQuantity < line.Quantity)
        {
            errors.Add('$"商品「{product.Name}」庫存不足（現有 {product.StockQuantity}，需求 {line.Quantity}）");
            continue;
        }
        product.StockQuantity -= line.Quantity;
        order.Items.Add(new OrderItem
        {
            ProductId = product.Id,
            Quantity = line.Quantity,
            UnitPriceSnapshot = product.UnitPrice
        });
    }
    return errors;
}

Why this shape (and not a rule-engine/pipeline)

CLAUDE.md: "small single-database internal system… Match that complexity level" and
"don't invent your own approach — follow convenabstraction
would be over-engineering here and the header rules are heterogeneous (customer is an async
DB lookup that yields a value; the rest are pura uniform rule
list models awkwardly. Two small private helpers behind the same public method is the
idiomatic, minimal move — consistent with the e

Verification

- dotnet test tests/OrderHub.Tests/OrderHub.Tesay green, with
no test edits (the 11 OrderServiceCreateTests are the behavior contract; Cancel/Pricing
tests also call CreateOrderAsync for setup). Exefore.
- Optionally build the Core project (dotnet build src/OrderHub.Core) to confirm compilation
independent of the running web app.

Commit

One independent commit containing only src/Ordece.cs, message
e.g. Refactor CreateOrderAsync validation into focused helpers (zh/en per repo style),
excluding the pre-existing unrelated working-trok logs).
