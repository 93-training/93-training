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

### 第二階段 — 自建 MCP Server:給 agent 造工具

練習 0

對比活動2練習0， AI agent使用playwright來測試建立訂單，之後給出測試步驟和結果及說明，當然執行的每一步驟都有詢問是否允許執行測試步驟

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

練習 1/2

已建立 OrderHub MCP Server + commit + run mcp inspector
- 三個 mcp 工正確列出
- 手動呼叫 LowStock(threshold=10),回傳的商品和 /Products 頁面上的低庫存商品一致
- 呼叫 GetOrder 用一個不存在的 Id,回應是清楚的錯誤訊息而不是 exception dump

練習 3

a. 未啟用mcp.json前，claude 主動詢問使用playwright的許可，以查看「哪些商品庫存低於 5?」。另外claude也注意到項目裡有OrderHub.Mcp這個專案，裡面的工具可以使用，並主動詢問是否需幫忙註冊，以下是對話

> help to check which products having stock quantity that less than 5

## claude > 
App isn't running anymore — the earlier instance is gone. Starting it.

  Read 2 files, ran 4 shell commands

● Background command "Wait until app responds on port 5150" completed (exit code 0)

App is up. Querying with threshold 5.

  Called playwright 2 times

5 products found. Let me also check whether any inactive products fall under 5 — the built-in query filters those out (p.IsActive && ...).

  Read 1 file, called playwright 2 times

庫存低於 5 的商品共 5 筆：

┌──────────┬───────────────────┬──────────┬──────────────┬────────┐
│   SKU    │       名稱        │ 現有庫存 │ 近 30 天銷量 │  狀態  │
├──────────┼───────────────────┼──────────┼──────────────┼────────┤
│ SKU-1048 │ 晨光 行動電源     │        2 │            7 │ 販售中 │
├──────────┼───────────────────┼──────────┼──────────────┼────────┤
│ SKU-1005 │ 極光 筆電支架     │        3 │            0 │ 販售中 │
├──────────┼───────────────────┼──────────┼──────────────┼────────┤
│ SKU-1023 │ 雲峰 27吋螢幕     │        3 │           13 │ 販售中 │
├──────────┼───────────────────┼──────────┼──────────────┼────────┤
│ SKU-1014 │ 星河 USB-C 集線器 │        4 │           14 │ 販售中 │
├──────────┼───────────────────┼──────────┼──────────────┼────────┤
│ SKU-1032 │ 曜石 機械鍵盤     │        4 │           18 │ 販售中 │
└──────────┴───────────────────┴──────────┴──────────────┴────────┘

資料來源是 app 內建的 /Products/LowStock?Threshold=5（ProductRepository.GetLowStockAsync → p.IsActive && p.StockQuantity < threshold，嚴格小於 5，依庫存升冪）。

兩點補充

我另外交叉核對了「已停售」的商品。 那支查詢有 p.IsActive 條件，所以停售品不會出現在結果裡。我掃過 /Products 全部 50 筆確認：3 筆停售商品（SKU-1009 / 1027 / 1041）庫存分別是 42 / 94 / 95，都遠高於 5 —— 所以這個過濾條件沒有漏掉任何低庫存商品，上面 5 筆就是完整答案。

若要看補貨急迫性，銷量欄比庫存欄更有訊息量。 SKU-1032（庫存 4、近 30 天賣 18）和 SKU-1023（庫存 3、賣 13）週轉最快；反觀 SKU-1005 庫存 3 但 30 天內完全沒賣出，急迫性低。我沒有改任何程式碼，這只是讀數解讀。

環境狀態

- Web app 是我這次啟動的，目前還在背景跑（port 5150，task bv5onjil4）。不需要的話跟我說我幫你停掉。
- Playwright 又在 repo 根目錄產生了 .playwright-mcp/（snapshot yml）。上次我要刪被拒絕，這次就沒再嘗試 —— 建議把它加進 .gitignore，免得每次都要清。

順帶一提：你剛建好的 OrderHub.Mcp 裡的 LowStock too0），但它還沒註冊到 Claude Code。要註冊的話：

claude mcp add orderhub -- dotnet run --project src

之後這類查詢就能直接問，不用開瀏覽器。要我幫你加嗎

b. 啟用mcp.json後，claude 會先驗證OrderHub.Mcp Server是否可以連接並返回正確數據，遇到設置問題會先解釋情況並給出調整建議

> yes, add the MCP server

## claude > 
  Ran 2 shell commands

Before registering, one thing to verify: the MCP project has no appsettings.json, so it falls back to the hardcoded Server=localhost — while the web app runs in Development against (localdb)\MSSQLLocalDB. Different instances. Let me test the server over stdio to see what it actually reaches.

  Made 1 scratchpad edit +5, ran 1 shell command
PostToolUse:Write says: PostToolUse hook fired: Write -> C:\Users\dm93\AppData\Local\Temp\2\claude\C--Users-dm93-source-repos-93-training-training-repo\4c6ad248-3c2e-4b05-9806-00ad49687ec8\scratchpad\mcp-probe.jsonl (logged to .claude/hooks/edit-log.txt)

Server works — same 5 products. Let me confirm it's actually the same database as the web app (matching seed data alone wouldn't prove it) by asking for order #207, which I created through the web app yesterday.

  Ran 2 shell commands

找不到訂單 207 — so the MCP server is on a different database than the web app. My concern was right; the low-stock numbers matched only because both DBs share the same fixed seed. Let me confirm #207 does exist in the web app's DB.

  Ran 1 shell command

Confirmed the mismatch. Let me verify the env-var override works before registering.

  Ran 1 shell command

Env override works — #207 returns correctly. Now registering.

  Ran 3 shell commands

Connected — but there's a conflict worth looking at: an orderhub server already existed at project scope. Let me check.

  Ran 1 shell command

.mcp.json exists (untracked) and already defines orderhub — so there are now two definitions. Let me test whether the project-scoped variant actually works, so I can recommend which to keep.

  Ran 2 shell commands

Registered and connected — but I need your call on one thing, because I found a pre-existing conflict.

What I verified:

┌────────────────────────────────────────────────────────────────────────┬───────────┬──────────────────────────┐
│                                Variant           ects? │    Reads correct DB?     │
├────────────────────────────────────────────────────────────────────────┼───────────┼──────────────────────────┤
│ Your .mcp.json (project scope) — dotnet run --pro      │ ✘ 找不到訂單 207         │
├────────────────────────────────────────────────────────────────────────┼───────────┼──────────────────────────┤
│ Mine (local scope) — prebuilt dll + ConnectionStr      │ ✔ returns #207 correctly │
└────────────────────────────────────────────────────────────────────────┴───────────┴──────────────────────────┘

The root cause: OrderHub.Mcp has no appsettings.json, so GetConnectionString("Default") returns null and Program.cs:18 falls back
to Server=localhost. But the web app runs in DeveloLLocalDB. Two different instances — the low-stocknumbers matched only because both share the same fixed seed, which is why I probed order #207 to tell them apart.

● User answered Claude's questions:
· 現在 orderhub 有兩個定義（project + local）會 mcp.json，刪掉 local（建議）

  Read 1 file

● Write(.mcp.json)
Added 5 lines, removed 2 lines
  "mcpServers": {
    "orderhub": {
      "command": "dotnet",
      "args": ["run", "--project", "src/OrderHub.Mcp"]
      "args": ["run", "--project", "src/Ord
      "env": {
        "ConnectionStrings__Default": "Servatabase=OrderHubTraining;Integrated Security=
True;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
      }
    }
  }
}
 No newline at end of file
}
PostToolUse:Write says: PostToolUse hook fired: e\repos\93-training\training-repo\.mcp.json (loggedto .claude/hooks/edit-log.txt)

  Ran 3 shell commands

Done. orderhub is now a single project-scoped server with the correct database.

Final state

orderhub: dotnet run --project src/OrderHub.Mcp - ⏸ Pending approval
playwright: npx @playwright/mcp@latest - ✔ Connecte

One step left that only you can do: project-scoped interactive approval (a safeguard, since .mcp.jsoncan arrive via a repo). Restart with claude and approve when prompted — after that the three tools are available:

┌─────────────────┬─────────────────────────────────────────┐
│      Tool       │                  用途
├─────────────────┼─────────────────────────────────────────┤
│ get_order       │ 依訂單 Id 查明細、折扣、應付總
├─────────────────┼─────────────────────────────────────────┤
│ low_stock       │ 列出庫存低於門檻的販售中商品（
├─────────────────┼─────────────────────────────────────────┤
│ customer_orders │ 查某客戶的全部訂單摘要
└─────────────────┴─────────────────────────────────────────┘

.mcp.json 的變更

只加了 env，command/args 保持原樣：

"env": {
  "ConnectionStrings__Default": "Server=(localdb)\\ubTraining;Integrated Security=True;..."
}

我沒有改成指向 dll —— 選項描述裡我提過那個做法，但實測後發現不需要：dotnet run 的 build 訊息是走 stderr，stdout 全程是乾淨的
JSON-RPC，所以協定不會被污染。保留 dotnet run 反而 0.0，程式碼改了會自動重建，分享給同學也不會壞。

驗證方式是用 dotnet run + 該 env var 實際跑一次 get陳志明 / Gold —— 不是只看它有沒有連上。

claude mcp remove orderhub -s local 已執行，scope

兩點提醒

.mcp.json 目前還是 untracked。 要跟課程小組分享就得Integrated Security，沒有帳密，可以安全進 git ——但它硬寫死 LocalDB，同學若用別的 SQL Server 實例會需要自行調整。要我幫你 commit 嗎？

練習 4

成功讓claude取消訂單，也是通過詢問操作許可，使用playwirght實現

> OrderHub.Web is running on background, now cancel an order #205 for me

## claude >

Web app is up on port 5150 (PID 24404). The cancel flow is a POST with antiforgery + a JS confirm, so I'll drive it through the browser.

  Called playwright 2 times

Order #205 confirmed — 待處理 (Pending), 客戶 周振宇 (銀卡), 1 × SKU-1008 極光行動電源, 應付 NT$3,135. Cancellable. Proceeding:

  Called playwright 3 times

Done. Order #205 is cancelled.

- Status: 待處理 → 已取消
- Success alert: 訂單 #205 已取消
- The 取消訂單 button is gone (CanCancel is now false)
- Order data unchanged as expected — 1 × SKU-1008 極光行動電源, 單價快照 NT$3,300, 銀卡 5% 折扣, 應付 NT$3,135

I went through the web UI at /Orders/Details/205 rather than the MCP tool, since we killed that server a moment ago.

One thing I did not verify: whether the 極光行動電源 stock was actually restored. CancelOrderAsync is supposed to restock, but I didn't capture the stock level before cancelling, so I have no baseline to compare against — and CLAUDE.md flags stock-restore-on-cancel as one of the exercise areas. Say the word and I'll check the current SKU-1008 stock, or read OrderService.CancelOrderAsync to see what it actually does.

### 這部分是讓claude取消訂單後去檢查庫存是否有增加
> then cancel order #202 for me

## claude >

  Called playwright 2 times

Order #202: 待處理, 郭俊傑 (金卡), 1 × SKU-1050 晨光桌上麥克風, 應付 NT$1,571.40. Let me grab the stock baseline first so I can settle the restore question this time.

  Searched for 1 pattern, called playwright 2 times

Baseline captured: SKU-1050 stock = 13. Order #202 is qty 1, so a working restore should land on 14. Cancelling now:

  Called playwright 4 times

Cancelled. Now the stock re-check:

  Called playwright 2 times

Order #202 is cancelled.

- Status: 待處理 → 已取消
- Alert: 訂單 #202 已取消
- 客戶 郭俊傑 (金卡會員), 1 × SKU-1050 晨光桌上麥克風, 應付 NT$1,571.40

And the stock-restore question from last time is now answered — it works. SKU-1050 went 13 → 14, exactly the cancelled quantity:

┌─────────────────────────┬────────┬───────┐
│           SKU           │ Before │ After │
├─────────────────────────┼────────┼───────┤
│ SKU-1050 晨光桌上麥克風 │ 13     │ 14    │
└─────────────────────────┴────────┴───────┘

One caveat on scope: this confirms restore works for a single-line, quantity-1, Pending order. The planted-bug areas CLAUDE.md mentions could still bite on multi-line orders, quantities > 1, or the Confirmed path — one passing case isn't proof the logic is correct in general. I didn't test those, and I didn't read OrderService.CancelOrderAsync to check the implementation.

SKU-1008 (from order #205) currently reads 51, but I have no baseline for it, so that number tells us nothing either way.

> cancel again #202 for me

## claude > 

Order #202 is already 已取消 from a moment ago — the UI no longer renders the 取消訂單 button, so there's nothing to click. But whether the server rejects a second cancel is worth knowing, since a broken guard would restore stock twice. Let me exercise it directly.

  Called playwright 2 times

The guard held. Verifying stock didn't move:

  Called playwright 2 times

#202 was not cancelled again — it was already cancelled, and the server correctly refused.

I couldn't do it through the UI (no 取消訂單 button on a cancelled order), so I POSTed to /Orders/Cancel/202 directly with a valid antiforgery token. The result:

狀態為 Cancelled 的訂單不可取消

Redirected back to /Orders/Details/202 with that as TempData["Error"] — the path at OrdersController.cs:106-109. No 500, no silent success.

Stock stayed at 14 — unchanged from the post-cancel value, so no double restore. That's the failure mode worth caring about here, and it isn't present.

So the state-transition guard in CancelOrderAsync holds for Cancelled → Cancelled. If you were probing for a planted bug in the double-cancel path, this one's clean. If you actually meant a different order number, tell me which and I'll do it.

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
