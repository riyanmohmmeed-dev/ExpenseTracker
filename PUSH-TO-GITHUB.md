# Push ExpenseTracker to GitHub

## 1. Create the repo on GitHub

- Go to [github.com/new](https://github.com/new).
- **Repository name:** `expense-tracker` (or `ExpenseTracker` / `personal-expense-tracker`).
- Description (optional): *Full-stack expense tracker with dashboard, budgets, recurring transactions, and CSV export. ASP.NET Core + Razor Pages + SQLite.*
- Choose **Public**, leave "Add a README" **unchecked** (you already have one).
- Click **Create repository**.

## 2. Push from your machine

From the **ExpenseTracker** folder (inside GitTime):

```bash
cd /Users/mohammedriyan/Projects/GitTime/ExpenseTracker

# If this folder isn’t a git repo yet:
git init
git add .
git commit -m "Initial commit: full-stack expense tracker with dashboard, budgets, recurring, export"

# Add your GitHub repo (replace YOUR_USERNAME with your GitHub username)
git remote add origin https://github.com/YOUR_USERNAME/expense-tracker.git

# Push (use main or master depending on your default branch)
git branch -M main
git push -u origin main
```

If ExpenseTracker is already part of the parent GitTime repo, you have two options:

- **Option A — Subfolder as its own repo:** Create a new folder, copy the ExpenseTracker contents into it, run `git init` there, then add, commit, and push as above.
- **Option B — Subtree or separate clone:** Use `git subtree split` or clone only the ExpenseTracker directory into a new repo and push that. (More advanced; Option A is simpler.)

## 3. After pushing

- Open `https://github.com/YOUR_USERNAME/expense-tracker` and confirm the README and screenshots (in `docs/`) look good.
- If you want to remove browser taskbars from the screenshots, crop the images in Preview (or any editor), save over the files in `docs/`, then commit and push again.
