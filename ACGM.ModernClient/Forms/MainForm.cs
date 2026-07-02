using ACGM.ModernClient.Configuration;
using ACGM.ModernClient.Models;
using ACGM.ModernClient.Protocol;
using ACGM.ModernClient.Logging;

namespace ACGM.ModernClient.Forms;

public sealed class MainForm : Form
{
    private Uri _endpoint;
    private string _characterName;
    private string _password;
    private readonly Dictionary<int, CharacterRecord> _charactersById = new();
    private readonly AcgmSettings _settings;
    private AcgmProtocolService _protocol;
    private CharacterDetails? _currentDetails;
    private CharacterRecord? _currentRecord;
    private string _guildName = string.Empty;
    private bool _useRescueSquad = true;
    private string _rescueSquadName = "Rescue Squad";
    private SplitContainer _mainSplit = null!;
    private bool _isApplyingSplitterDistance;

    private readonly MenuStrip _menuStrip = new();
    private readonly TabControl _mainTabs = new() { Dock = DockStyle.Fill };
    private readonly TreeView _treeView = new() { Dock = DockStyle.Fill, HideSelection = false };
    private readonly ImageList _treeImages = new() { ImageSize = new Size(16, 16), ColorDepth = ColorDepth.Depth32Bit };
    private readonly ListView _flatList = new() { Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true };
    private readonly TabControl _detailTabs = new() { Dock = DockStyle.Fill };
    private readonly StatusStrip _statusStrip = new();
    private readonly ToolStripStatusLabel _statusMain = new("Ready") { Spring = false, Width = 200, TextAlign = ContentAlignment.MiddleLeft };
    private readonly ToolStripStatusLabel _statusConnection = new("Disconnected") { Spring = false, Width = 260, TextAlign = ContentAlignment.MiddleLeft };
    private readonly ToolStripStatusLabel _statusBytes = new("") { Spring = false, Width = 160, TextAlign = ContentAlignment.MiddleLeft };
    private ToolStripMenuItem _administratorMenu = null!;
    private int _currentSecurityLevel = 1;

    private TextBox _nameText = null!;
    private TextBox _levelText = null!;
    private TextBox _classText = null!;
    private ComboBox _rankCombo = null!;
    private ComboBox _computedRankCombo = null!;
    private ComboBox _raceCombo = null!;
    private ComboBox _sexCombo = null!;
    private RadioButton _muleYes = null!;
    private RadioButton _muleNo = null!;
    private TextBox _muleForText = null!;
    private TextBox _mainCharacterText = null!;
    private TextBox _lifestoneText = null!;
    private TextBox _tiedToText = null!;
    private CheckBox _canSummonCheck = null!;
    private CheckBox _pkCheck = null!;
    private CheckBox _rescueCheck = null!;
    private CheckBox _hideInfoCheck = null!;
    private Label _lastModifiedLabel = null!;
    private TextBox _bioText = null!;

    private TextBox _realNameText = null!;
    private TextBox _cityStateText = null!;
    private TextBox _miscRealLifeText = null!;
    private TextBox _emailText = null!;
    private TextBox _icqText = null!;

    private TextBox _awardsText = null!;
    private ListView _rescueSquadList = null!;
    private ListView _portalList = null!;
    private ListView _tradeSkillsList = null!;
    private ComboBox _searchRankCombo = null!;
    private ComboBox _searchRaceCombo = null!;
    private DataGridView _searchSkillGrid = null!;
    private ListView _searchResultsList = null!;
    private readonly Dictionary<int, CharacterDetails> _searchDetailsCache = new();

    private TextBox _allegianceCharacterText = null!;
    private TextBox _allegiancePatronText = null!;
    private TextBox _allegianceMonarchText = null!;
    private TextBox _allegiancePathText = null!;
    private TextBox _allegianceDirectCountText = null!;
    private TextBox _allegianceTotalCountText = null!;
    private ListView _directVassalsList = null!;
    private TreeView _immediateTreeView = null!;
    private TextBox _allegianceNotesText = null!;

    private TabControl _skillsTabs = null!;
    private ListBox _specSkillsList = null!;
    private ListBox _trainedSkillsList = null!;
    private ListBox _untrainedSkillsList = null!;
    private ListBox _unusableSkillsList = null!;
    private ListView _skillValuesList = null!;
    private Button _lowerSpecButton = null!;
    private Button _raiseTrainedButton = null!;
    private Button _lowerTrainedButton = null!;
    private Button _raiseUntrainedButton = null!;
    private Button _lowerUntrainedButton = null!;
    private Button _raiseUnusableButton = null!;
    private readonly List<SkillInfo> _currentSkills = new();

    private Button _allegianceSaveButton = null!;
    private Button _allegianceResetButton = null!;
    private Button _changePatronButton = null!;
    private Button _addVassalButton = null!;
    private Button _removeVassalButton = null!;
    private readonly List<string> _pendingAddedVassals = new();
    private readonly List<CharacterRecord> _pendingRemovedVassals = new();
    private CharacterRecord? _pendingNewPatron;
    private CharacterRecord? _allegianceCharacter;
    private AllegianceInfo? _currentAllegianceInfo;

    private CharacterRecord? SelectedCharacter => _treeView.SelectedNode?.Tag as CharacterRecord;
    private bool _windowStateReady;

    public MainForm(Uri endpoint, string characterName, string password)
    {
        _endpoint = endpoint;
        _characterName = characterName;
        _password = password;
        _settings = AcgmSettings.Load();
        _protocol = new AcgmProtocolService(_endpoint, _characterName, _password);

        UpdateWindowTitle();
        Width = Math.Max(820, _settings.MainWindowWidth);
        Height = Math.Max(580, _settings.MainWindowHeight);
        MinimumSize = new Size(820, 580);
        StartPosition = FormStartPosition.CenterScreen;

        BuildMenu();
        BuildMainLayout();

        Shown += MainForm_Shown;
        FormClosing += (_, _) => SaveWindowState();
        ResizeEnd += (_, _) => SaveWindowState();
        Move += (_, _) => { if (_windowStateReady && WindowState == FormWindowState.Normal) SaveWindowState(); };
    }

    private void BuildMenu()
    {
        _menuStrip.Items.Clear();

        var fileMenu = new ToolStripMenuItem("File");
        fileMenu.DropDownItems.Add(CreateMenuItem("Login", async (_, _) => await LoginAgainAsync()));
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add(CreateMenuItem("Refresh Tree", async (_, _) => await RefreshTreeAsync()));
        fileMenu.DropDownItems.Add(CreateMenuItem("Save Current Character", async (_, _) => await SaveCurrentCharacterAsync()));
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add(CreateMenuItem("Exit", (_, _) => Close()));

        var currentPlayerMenu = new ToolStripMenuItem("Current Player Functions");
        currentPlayerMenu.DropDownItems.Add(CreateMenuItem("Refresh Character Info", async (_, _) => await LoadSelectedCharacterAsync()));
        currentPlayerMenu.DropDownItems.Add(CreateMenuItem("Save Character Info", async (_, _) => await SaveCurrentCharacterAsync()));
        currentPlayerMenu.DropDownItems.Add(new ToolStripSeparator());
        currentPlayerMenu.DropDownItems.Add(CreateMenuItem("Add Vassal", async (_, _) => await CurrentPlayerAddVassalAsync()));
        currentPlayerMenu.DropDownItems.Add(CreateMenuItem("Change Patron", async (_, _) => await CurrentPlayerChangePatronAsync()));
        currentPlayerMenu.DropDownItems.Add(CreateMenuItem("Change Password", async (_, _) => await CurrentPlayerChangePasswordAsync()));

        var treeMenu = new ToolStripMenuItem("Tree Operations");
        treeMenu.DropDownItems.Add(CreateMenuItem("Refresh Tree", async (_, _) => await RefreshTreeAsync()));
        treeMenu.DropDownItems.Add(CreateMenuItem("Find Character", async (_, _) => await FindCharacterAsync()));
        treeMenu.DropDownItems.Add(new ToolStripSeparator());
        treeMenu.DropDownItems.Add(CreateMenuItem("Expand All", (_, _) => _treeView.ExpandAll()));
        treeMenu.DropDownItems.Add(CreateMenuItem("Collapse All", (_, _) => _treeView.CollapseAll()));

        var optionsMenu = new ToolStripMenuItem("Options");
        optionsMenu.DropDownItems.Add(CreateMenuItem("Save Window Layout", (_, _) => SaveWindowLayoutNow()));
        optionsMenu.DropDownItems.Add(CreateMenuItem("Reset Window Layout", (_, _) => ResetWindowLayout()));
        optionsMenu.DropDownItems.Add(CreateMenuItem("Open Logs Folder", (_, _) => OpenLogsFolder()));

        _administratorMenu = new ToolStripMenuItem("Administrator");
        _administratorMenu.DropDownItems.Add(CreateMenuItem("Backup Database", async (_, _) => await AdminBackupDatabaseAsync()));
        _administratorMenu.DropDownItems.Add(CreateMenuItem("Change Character's Security Level", async (_, _) => await AdminChangeSecurityLevelAsync()));
        _administratorMenu.DropDownItems.Add(CreateMenuItem("Reset Character's Password", async (_, _) => await AdminResetCharacterPasswordAsync()));
        _administratorMenu.DropDownItems.Add(CreateMenuItem("Server Setup", async (_, _) => await AdminServerSetupAsync()));
        UpdateAdministratorMenuVisibility();

        var helpMenu = new ToolStripMenuItem("Help");
        helpMenu.DropDownItems.Add(CreateMenuItem("About ACGM Modern Client", (_, _) => ShowAboutDialog()));
        helpMenu.DropDownItems.Add(CreateMenuItem("Open Logs Folder", (_, _) => OpenLogsFolder()));
        helpMenu.DropDownItems.Add(CreateMenuItem("Protocol Diagnostics Folder", (_, _) => OpenDiagnosticsFolder()));

        _menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, currentPlayerMenu, treeMenu, optionsMenu, _administratorMenu, helpMenu });
        MainMenuStrip = _menuStrip;
        Controls.Add(_menuStrip);
    }

    private static ToolStripMenuItem CreateMenuItem(string text, EventHandler onClick)
    {
        var item = new ToolStripMenuItem(text);
        item.Click += onClick;
        return item;
    }

    private void UpdateAdministratorMenuVisibility()
    {
        if (_administratorMenu == null)
            return;

        // Legacy VB6 behavior: mnuAdministrator.Visible is true only when
        // iCurSecurityLevel = 3. Normal users should not see administrator tools.
        _administratorMenu.Visible = _currentSecurityLevel == 3;
    }

    private async Task RefreshLoginSecurityAsync()
    {
        try
        {
            var login = await _protocol.LoginAsync();
            _currentSecurityLevel = login.SecurityLevel;
            UpdateAdministratorMenuVisibility();
        }
        catch (Exception ex)
        {
            _currentSecurityLevel = 1;
            UpdateAdministratorMenuVisibility();
            AcgmLogger.TryWrite("Login/security refresh failed: " + ex);
        }
    }


    private async Task LoginAgainAsync()
    {
        var response = DialogResult.Yes;
        if (_currentDetails != null && _currentRecord != null)
        {
            response = MessageBox.Show(this,
                "Login as another character? Any unsaved changes in the current character editor will be discarded.",
                "ACGM Login", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        if (response != DialogResult.Yes)
            return;

        using var login = new PlayerLoginForm(_settings);
        if (login.ShowDialog(this) != DialogResult.OK)
            return;

        _endpoint = login.ServerEndpoint;
        _characterName = login.CharacterName;
        _password = login.Password;
        _protocol = new AcgmProtocolService(_endpoint, _characterName, _password);

        _charactersById.Clear();
        _currentDetails = null;
        _currentRecord = null;
        _currentAllegianceInfo = null;
        _allegianceCharacter = null;
        _pendingAddedVassals.Clear();
        _pendingRemovedVassals.Clear();
        _pendingNewPatron = null;

        UpdateWindowTitle();
        _statusMain.Text = "Logged in";
        _statusConnection.Text = $"Logged in as {_characterName}";
        _statusBytes.Text = string.Empty;

        await RefreshLoginSecurityAsync();
        await RefreshTreeAsync();
    }

    private void UpdateWindowTitle()
    {
        var guildName = string.IsNullOrWhiteSpace(_guildName) ? "The Regs" : _guildName.Trim();
        Text = $"ACGM ({guildName}) - Logged in as {_characterName}";
    }

    private void BuildMainLayout()
    {
        var treePage = new TabPage("Allegiance Tree");
        var flatPage = new TabPage("Flat Listing");
        var rescuePage = new TabPage("Rescue Squad");
        var summonPage = new TabPage("Summonable Portal List");
        var tradePage = new TabPage("Trade Skills List");
        var searchPage = new TabPage("Search");

        _mainTabs.TabPages.AddRange(new[] { treePage, flatPage, rescuePage, summonPage, tradePage, searchPage });

        _mainSplit = new SplitContainer
        {
            Dock = DockStyle.Fill,
            FixedPanel = FixedPanel.None,
            SplitterWidth = 4,
            // Min sizes are applied after the form is shown. Setting Panel2MinSize
            // during early construction can make WinForms call SplitterDistance
            // while the SplitContainer still has width 0, which throws.
            Panel1MinSize = 1,
            Panel2MinSize = 1
        };
        _mainSplit.SplitterMoved += (_, _) => SaveSplitterDistance();
        _mainSplit.Resize += (_, _) => ClampCurrentSplitterDistance();

        var leftPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4, 3, 4, 3) };
        var titleLabel = new Label
        {
            Text = "Allegiance Tree",
            Dock = DockStyle.Top,
            Height = 18,
            Font = new Font(Font, FontStyle.Bold)
        };
        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 36,
            FlowDirection = FlowDirection.LeftToRight
        };
        var refreshButton = new Button { Text = "Refresh Tree", Width = 110, Height = 28 };
        var findButton = new Button { Text = "Find Character", Width = 110, Height = 28 };
        refreshButton.Click += async (_, _) => await RefreshTreeAsync();
        findButton.Click += async (_, _) => await FindCharacterAsync();
        buttonPanel.Controls.Add(refreshButton);
        buttonPanel.Controls.Add(findButton);

        LoadTreeImages();
        _treeView.BorderStyle = BorderStyle.Fixed3D;
        _treeView.ImageList = _treeImages;
        _treeView.AfterSelect += async (_, _) => await LoadSelectedCharacterAsync();
        leftPanel.Controls.Add(_treeView);
        leftPanel.Controls.Add(buttonPanel);
        leftPanel.Controls.Add(titleLabel);
        _mainSplit.Panel1.Controls.Add(leftPanel);

        BuildDetailTabs();
        _mainSplit.Panel2.Controls.Add(_detailTabs);
        treePage.Controls.Add(_mainSplit);

        _flatList.Columns.Add("Name", 220);
        _flatList.Columns.Add("Level", 80);
        _flatList.Columns.Add("Patron", 180);
        flatPage.Controls.Add(_flatList);

        BuildRescueSquadTab(rescuePage);
        BuildPortalListTab(summonPage);
        BuildTradeSkillsListTab(tradePage);
        BuildSearchTab(searchPage);

        _statusStrip.Items.Add(_statusMain);
        _statusStrip.Items.Add(_statusConnection);
        _statusStrip.Items.Add(_statusBytes);

        Controls.Add(_mainTabs);
        Controls.Add(_statusStrip);
        _mainTabs.BringToFront();
    }

    private async void MainForm_Shown(object? sender, EventArgs e)
    {
        try
        {
            RestoreWindowState();
            if (_settings.SelectedDetailTabIndex >= 0 && _settings.SelectedDetailTabIndex < _detailTabs.TabPages.Count)
                _detailTabs.SelectedIndex = _settings.SelectedDetailTabIndex;
            _windowStateReady = true;
            // SplitContainer can reject SplitterDistance while the form is still
            // being laid out. Defer the 25/75 or saved value until the control
            // has a real width, then clamp it to the active min-size bounds.
            BeginInvoke(new Action(ApplyInitialSplitterDistance));
            await RefreshLoginSecurityAsync();
            await RefreshTreeAsync();
        }
        catch (Exception ex)
        {
            _statusMain.Text = "Startup error";
            _statusConnection.Text = ex.Message;
            MessageBox.Show(this, ex.ToString(), "ACGM Modern Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ApplyInitialSplitterDistance()
    {
        if (!_mainSplit.IsHandleCreated || _mainSplit.Width <= 0)
            return;

        ApplyRealSplitterMinimums();

        var desired = _settings.MainSplitterDistance > 0
            ? _settings.MainSplitterDistance
            : (int)Math.Round(_mainSplit.ClientSize.Width * 0.25);

        SetSplitterDistanceSafely(desired);
    }

    private void ClampCurrentSplitterDistance()
    {
        if (!_mainSplit.IsHandleCreated || _mainSplit.Width <= 0)
            return;

        ApplyRealSplitterMinimums();
        SetSplitterDistanceSafely(_mainSplit.SplitterDistance);
    }

    private void ApplyRealSplitterMinimums()
    {
        // Only apply the true legacy-style minimum sizes once the SplitContainer
        // has a usable width. This avoids the startup crash seen when Panel2MinSize
        // was assigned during BuildMainLayout() before WinForms completed layout.
        var desiredPanel1Min = 160;
        var desiredPanel2Min = 320;

        if (_mainSplit.ClientSize.Width < desiredPanel1Min + desiredPanel2Min + _mainSplit.SplitterWidth)
            return;

        if (_mainSplit.Panel1MinSize != desiredPanel1Min)
            _mainSplit.Panel1MinSize = desiredPanel1Min;

        if (_mainSplit.Panel2MinSize != desiredPanel2Min)
            _mainSplit.Panel2MinSize = desiredPanel2Min;
    }

    private void SetSplitterDistanceSafely(int desired)
    {
        var min = _mainSplit.Panel1MinSize;
        var max = _mainSplit.ClientSize.Width - _mainSplit.Panel2MinSize - _mainSplit.SplitterWidth;

        // If the window is temporarily too small during startup/layout, do not
        // force a value. WinForms will complete layout and Resize will clamp it.
        if (max < min)
            return;

        var safe = Math.Max(min, Math.Min(max, desired));

        if (_mainSplit.SplitterDistance == safe)
            return;

        try
        {
            _isApplyingSplitterDistance = true;
            _mainSplit.SplitterDistance = safe;
        }
        finally
        {
            _isApplyingSplitterDistance = false;
        }
    }

    private void SaveSplitterDistance()
    {
        if (_isApplyingSplitterDistance || _mainSplit.Width <= 0 || _mainSplit.SplitterDistance <= 0)
            return;

        _settings.MainSplitterDistance = _mainSplit.SplitterDistance;
        _settings.Save();
    }


    private void BuildRescueSquadTab(TabPage page)
    {
        var root = BuildUtilityTabRoot(page, "Rescue Squad", "Refresh Rescue Squad", async () => await RefreshRescueSquadAsync());
        _rescueSquadList = CreateUtilityListView();
        AddColumns(_rescueSquadList,
            ("Name", 160), ("Level", 70), ("Lifestone", 150), ("Tied To", 150),
            ("Can Summon", 90), ("Main Character Name", 160), ("Email Address", 190), ("ICQ Number", 110));
        _rescueSquadList.DoubleClick += async (_, _) => await SelectUtilityCharacterAsync(_rescueSquadList);
        root.Controls.Add(_rescueSquadList, 0, 0);
    }

    private void BuildPortalListTab(TabPage page)
    {
        var root = BuildUtilityTabRoot(page, "Summonable Portal List", "Refresh Portal List", async () => await RefreshPortalListAsync());
        _portalList = CreateUtilityListView();
        AddColumns(_portalList, ("Name", 180), ("Level", 80), ("Tied To", 220), ("Lifestone", 220));
        _portalList.DoubleClick += async (_, _) => await SelectUtilityCharacterAsync(_portalList);
        root.Controls.Add(_portalList, 0, 0);
    }

    private void BuildTradeSkillsListTab(TabPage page)
    {
        var root = BuildUtilityTabRoot(page, "Trade Skill List", "Get Trade Skills", async () => await RefreshTradeSkillsListAsync());
        _tradeSkillsList = CreateUtilityListView();
        AddColumns(_tradeSkillsList, ("Name", 180), ("Level", 80), ("Alchemy", 110), ("Cooking", 110), ("Fletching", 110));
        _tradeSkillsList.DoubleClick += async (_, _) => await SelectUtilityCharacterAsync(_tradeSkillsList);
        root.Controls.Add(_tradeSkillsList, 0, 0);
    }

    private static TableLayoutPanel BuildUtilityTabRoot(TabPage page, string frameTitle, string buttonText, Func<Task> refreshAction)
    {
        var frame = new GroupBox
        {
            Text = frameTitle,
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };
        page.Controls.Add(frame);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        frame.Controls.Add(root);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0)
        };
        var refreshButton = new Button { Text = buttonText, Width = 150, Height = 30 };
        refreshButton.Click += async (_, _) => await refreshAction();
        buttonPanel.Controls.Add(refreshButton);
        root.Controls.Add(buttonPanel, 0, 1);
        return root;
    }

    private static ListView CreateUtilityListView() => new()
    {
        Dock = DockStyle.Fill,
        View = View.Details,
        FullRowSelect = true,
        GridLines = true,
        HideSelection = false,
        MultiSelect = false
    };

    private static void AddColumns(ListView list, params (string Header, int Width)[] columns)
    {
        foreach (var (header, width) in columns)
            list.Columns.Add(header, width);
    }

    private void BuildSearchTab(TabPage page)
    {
        // 0.11.16: restored legacy VB6 Search workspace UI.
        // Milestone 2 wires the restored controls to local search execution.
        var frame = new GroupBox
        {
            Text = "Search",
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };
        page.Controls.Add(frame);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        frame.Controls.Add(root);

        var filterBox = new GroupBox
        {
            Text = "Search Filter",
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };
        root.Controls.Add(filterBox, 0, 0);

        var filterLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 6
        };
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 58));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        filterLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        filterLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        filterLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        filterLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
        filterLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        filterLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        filterBox.Controls.Add(filterLayout);

        var rankLabel = new Label { Text = "Rank:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold) };
        _searchRankCombo = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        PopulateSearchRankCombo();
        filterLayout.Controls.Add(rankLabel, 0, 0);
        filterLayout.Controls.Add(_searchRankCombo, 1, 0);

        var raceLabel = new Label { Text = "Race:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold) };
        _searchRaceCombo = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        PopulateSearchRaceCombo();
        filterLayout.Controls.Add(raceLabel, 0, 1);
        filterLayout.Controls.Add(_searchRaceCombo, 1, 1);

        _searchSkillGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = SystemColors.Window,
            BorderStyle = BorderStyle.Fixed3D,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
            EditMode = DataGridViewEditMode.EditOnEnter,
            MultiSelect = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.CellSelect
        };
        _searchSkillGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Skill",
            Name = "Skill",
            ReadOnly = true,
            FillWeight = 72
        });
        _searchSkillGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Min",
            Name = "Minimum",
            FillWeight = 28
        });
        PopulateSearchSkillGrid();
        filterLayout.Controls.Add(_searchSkillGrid, 0, 2);
        filterLayout.SetColumnSpan(_searchSkillGrid, 2);

        var searchButton = new Button { Text = "Search", Dock = DockStyle.Fill, Height = 32 };
        searchButton.Click += async (_, _) => await ExecuteSearchAsync(searchButton);
        filterLayout.Controls.Add(searchButton, 0, 4);
        filterLayout.SetColumnSpan(searchButton, 2);

        var resetButton = new Button { Text = "Reset", Dock = DockStyle.Fill, Height = 32 };
        resetButton.Click += (_, _) => ResetSearchFilters();
        filterLayout.Controls.Add(resetButton, 0, 5);
        filterLayout.SetColumnSpan(resetButton, 2);

        _searchResultsList = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            HideSelection = false,
            MultiSelect = false
        };
        AddColumns(_searchResultsList, ("Name", 180), ("Level", 70), ("Rank", 100), ("Race", 110));
        root.Controls.Add(_searchResultsList, 1, 0);
    }

    private void PopulateSearchRaceCombo()
    {
        _searchRaceCombo.BeginUpdate();
        _searchRaceCombo.Items.Clear();
        _searchRaceCombo.Items.Add(new LegacyChoice("", "Any"));
        foreach (var choice in LegacyCharacterParser.RaceChoices.Where(choice => choice.Value != "0"))
            _searchRaceCombo.Items.Add(choice);
        _searchRaceCombo.EndUpdate();
        _searchRaceCombo.SelectedIndex = 0;
    }

    private void PopulateSearchRankCombo()
    {
        _searchRankCombo.BeginUpdate();
        _searchRankCombo.Items.Clear();
        _searchRankCombo.Items.Add(new LegacyChoice("", "Any"));
        for (var i = 1; i <= 10; i++)
            _searchRankCombo.Items.Add(new LegacyChoice(i.ToString(), i.ToString()));
        _searchRankCombo.EndUpdate();
        _searchRankCombo.SelectedIndex = 0;
    }

    private void PopulateSearchSkillGrid()
    {
        _searchSkillGrid.Rows.Clear();
        foreach (var skill in LegacySkillParser.SkillDefinitions)
            _searchSkillGrid.Rows.Add(skill.Name, "0");
    }

    private void ResetSearchFilters()
    {
        if (_searchRankCombo.Items.Count > 0)
            _searchRankCombo.SelectedIndex = 0;
        if (_searchRaceCombo.Items.Count > 0)
            _searchRaceCombo.SelectedIndex = 0;

        foreach (DataGridViewRow row in _searchSkillGrid.Rows)
            row.Cells[1].Value = "0";

        _searchResultsList.Items.Clear();
        _statusMain.Text = "Search filters reset";
    }

    private async Task ExecuteSearchAsync(Button searchButton)
    {
        if (_charactersById.Count == 0)
        {
            MessageBox.Show(this, "The allegiance tree is not loaded yet. Refresh the tree and try again.", "ACGM Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!TryReadSearchCriteria(out var rankFilter, out var raceFilter, out var skillMinimums))
            return;

        _statusMain.Text = "Searching characters";
        _statusConnection.Text = "Loading character details";
        _statusBytes.Text = string.Empty;
        Cursor = Cursors.WaitCursor;
        searchButton.Enabled = false;

        try
        {
            var results = new List<(CharacterRecord Record, CharacterDetails Details)>();
            var totalBytes = 0;

            foreach (var record in _charactersById.Values.OrderBy(r => r.Name).ThenBy(r => r.Id))
            {
                var (details, bytes) = await GetSearchCharacterDetailsAsync(record);
                totalBytes += bytes;

                if (!MatchesSearchCriteria(details, rankFilter, raceFilter, skillMinimums))
                    continue;

                results.Add((record, details));
            }

            PopulateSearchResults(results);
            _statusMain.Text = $"Search complete: {results.Count} match{(results.Count == 1 ? string.Empty : "es")}";
            _statusConnection.Text = "Closing Connection";
            _statusBytes.Text = totalBytes > 0 ? $"{totalBytes} bytes received" : string.Empty;
        }
        catch (Exception ex)
        {
            _statusMain.Text = "Search error";
            _statusConnection.Text = ex.Message;
            AcgmLogger.TryWrite(ex.ToString());
            MessageBox.Show(this, ex.ToString(), "ACGM Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            searchButton.Enabled = true;
            Cursor = Cursors.Default;
        }
    }

    private bool TryReadSearchCriteria(out string rankFilter, out string raceFilter, out Dictionary<string, int> skillMinimums)
    {
        rankFilter = GetSelectedLegacyValue(_searchRankCombo);
        raceFilter = GetSelectedLegacyValue(_searchRaceCombo);
        skillMinimums = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (DataGridViewRow row in _searchSkillGrid.Rows)
        {
            if (row.IsNewRow)
                continue;

            var skillName = Convert.ToString(row.Cells[0].Value)?.Trim() ?? string.Empty;
            var rawValue = Convert.ToString(row.Cells[1].Value)?.Trim() ?? "0";
            if (string.IsNullOrWhiteSpace(rawValue))
                rawValue = "0";

            if (!int.TryParse(rawValue, out var minimum) || minimum < 0)
            {
                MessageBox.Show(this, $"Enter a non-negative number for {skillName}.", "ACGM Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _searchSkillGrid.CurrentCell = row.Cells[1];
                return false;
            }

            row.Cells[1].Value = minimum.ToString();
            if (minimum > 0 && skillName.Length > 0)
                skillMinimums[skillName] = minimum;
        }

        return true;
    }

    private async Task<(CharacterDetails Details, int Bytes)> GetSearchCharacterDetailsAsync(CharacterRecord record)
    {
        if (_searchDetailsCache.TryGetValue(record.Id, out var cached))
            return (cached, 0);

        var result = await _protocol.GetCharacterInfoAsync(record.Id);
        _searchDetailsCache[record.Id] = result.Details;
        return result;
    }

    private static bool MatchesSearchCriteria(CharacterDetails details, string rankFilter, string raceFilter, IReadOnlyDictionary<string, int> skillMinimums)
    {
        if (!string.IsNullOrWhiteSpace(rankFilter) && !string.Equals(details.Rank, rankFilter, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrWhiteSpace(raceFilter) && !string.Equals(details.Race, raceFilter, StringComparison.OrdinalIgnoreCase))
            return false;

        if (skillMinimums.Count == 0)
            return true;

        var skillsByName = LegacySkillParser.Parse(details.Skills)
            .ToDictionary(skill => skill.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var (skillName, minimum) in skillMinimums)
        {
            if (!skillsByName.TryGetValue(skillName, out var skill) || skill.Value < minimum)
                return false;
        }

        return true;
    }

    private void PopulateSearchResults(IEnumerable<(CharacterRecord Record, CharacterDetails Details)> results)
    {
        _searchResultsList.BeginUpdate();
        _searchResultsList.Items.Clear();

        foreach (var (record, details) in results.OrderBy(r => r.Record.Name).ThenBy(r => r.Record.Id))
        {
            var item = new ListViewItem(record.Name) { Tag = record };
            item.SubItems.Add(FirstNonEmpty(details.Level, record.Level.ToString()));
            item.SubItems.Add(GetRankDisplay(details));
            item.SubItems.Add(GetRaceDisplay(details.Race));
            _searchResultsList.Items.Add(item);
        }

        _searchResultsList.EndUpdate();
    }

    private static string GetSelectedLegacyValue(ComboBox combo)
    {
        return combo.SelectedItem is LegacyChoice choice ? choice.Value : string.Empty;
    }

    private static string GetRankDisplay(CharacterDetails details)
    {
        var choices = LegacyCharacterParser.GetRankChoices(details.Race, details.Sex);
        var match = choices.FirstOrDefault(choice => string.Equals(choice.Value, details.Rank, StringComparison.OrdinalIgnoreCase));
        return match == null || match.Value == "0" ? details.Rank : match.Text;
    }

    private static string GetRaceDisplay(string raceValue)
    {
        var match = LegacyCharacterParser.RaceChoices.FirstOrDefault(choice => string.Equals(choice.Value, raceValue, StringComparison.OrdinalIgnoreCase));
        return match == null || match.Value == "0" ? raceValue : match.Text;
    }

    private static void AddPlaceholder(TabPage page, string text)
    {
        page.Controls.Add(new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        });
    }

    private void BuildDetailTabs()
    {
        var basicPage = new TabPage("Basic Info");
        var allegiancePage = new TabPage("Allegiance Info");
        var skillsPage = new TabPage("Skills");
        var realLifePage = new TabPage("Real Life & Contact Info");
        var awardsPage = new TabPage("Awards");
        _detailTabs.TabPages.AddRange(new[] { basicPage, allegiancePage, skillsPage, realLifePage, awardsPage });
        _detailTabs.SelectedIndexChanged += (_, _) => SaveSelectedTab();

        // 0.3.1 layout note:
        // The first 0.3 build used right-anchored text boxes inside a narrow GroupBox.
        // On wide monitors that made controls stretch over the labels, causing the
        // Character Info values to appear clipped/hidden.  This patch uses a
        // TableLayoutPanel so labels and fields keep their own columns.
        var basic = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(4, 3, 4, 3),
            ColumnCount = 1,
            RowCount = 3
        };
        basic.RowStyles.Add(new RowStyle(SizeType.Absolute, 352));
        basic.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        basic.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        basicPage.Controls.Add(basic);

        var characterBox = new GroupBox
        {
            Text = "Character Info",
            Dock = DockStyle.Fill,
            Padding = new Padding(7)
        };
        basic.Controls.Add(characterBox, 0, 0);

        var characterGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 12,
            Padding = new Padding(3)
        };
        characterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 132));
        characterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        characterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 84));
        characterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        characterGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108));
        for (var i = 0; i < 12; i++)
            characterGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, i == 10 ? 34 : 28));
        characterBox.Controls.Add(characterGrid);

        _nameText = AddGridTextRow(characterGrid, "Name:", 0, 1, 4);
        _levelText = AddGridTextRow(characterGrid, "Level:", 1, 1, 4);
        _classText = AddGridTextRow(characterGrid, "Class:", 2, 1, 4);
        _rankCombo = AddGridComboRow(characterGrid, "Rank:", 3, 1, 2);
        _computedRankCombo = AddGridComboRow(characterGrid, "Computed Rank:", 4, 1, 2);

        AddGridLabel(characterGrid, "Race:", 5, 0);
        _raceCombo = CreateComboBox();
        characterGrid.Controls.Add(_raceCombo, 1, 5);
        characterGrid.SetColumnSpan(_raceCombo, 1);
        AddGridLabel(characterGrid, "Sex:", 5, 2);
        _sexCombo = CreateComboBox();
        characterGrid.Controls.Add(_sexCombo, 3, 5);
        characterGrid.SetColumnSpan(_sexCombo, 1);

        AddGridLabel(characterGrid, "Is Mule:", 6, 0);
        var mulePanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };
        _muleYes = new RadioButton { Text = "Yes", AutoSize = true, Margin = new Padding(0, 4, 12, 0) };
        _muleNo = new RadioButton { Text = "No", AutoSize = true, Margin = new Padding(0, 4, 0, 0) };
        mulePanel.Controls.AddRange(new Control[] { _muleYes, _muleNo });
        characterGrid.Controls.Add(mulePanel, 1, 6);
        AddGridLabel(characterGrid, "Mule for:", 6, 2);
        _muleForText = CreateTextBox();
        characterGrid.Controls.Add(_muleForText, 3, 6);
        characterGrid.SetColumnSpan(_muleForText, 2);

        _mainCharacterText = AddGridTextRow(characterGrid, "Main Character Name:", 7, 1, 4);
        _lifestoneText = AddGridTextRow(characterGrid, "Lifestoned At:", 8, 1, 4);

        AddGridLabel(characterGrid, "Tied To:", 9, 0);
        _tiedToText = CreateTextBox();
        characterGrid.Controls.Add(_tiedToText, 1, 9);
        characterGrid.SetColumnSpan(_tiedToText, 3);
        _canSummonCheck = new CheckBox { Text = "Can Summon", Dock = DockStyle.Fill, AutoSize = true };
        characterGrid.Controls.Add(_canSummonCheck, 4, 9);

        var flagsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };
        _pkCheck = new CheckBox { Text = "PK", AutoSize = true, Margin = new Padding(0, 6, 18, 0) };
        _rescueCheck = new CheckBox { Text = "Rescue Squad Member", AutoSize = true, Margin = new Padding(0, 6, 18, 0) };
        _hideInfoCheck = new CheckBox { Text = "Hide Info on Web", AutoSize = true, Margin = new Padding(0, 6, 0, 0) };
        flagsPanel.Controls.AddRange(new Control[] { _pkCheck, _rescueCheck, _hideInfoCheck });
        characterGrid.Controls.Add(flagsPanel, 1, 10);
        characterGrid.SetColumnSpan(flagsPanel, 4);

        _lastModifiedLabel = new Label
        {
            Text = "Last Modified:",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font(Font, FontStyle.Bold),
            AutoEllipsis = true
        };
        characterGrid.Controls.Add(_lastModifiedLabel, 0, 11);
        characterGrid.SetColumnSpan(_lastModifiedLabel, 5);

        var bioBox = new GroupBox
        {
            Text = "Bio",
            Dock = DockStyle.Fill,
            Padding = new Padding(7)
        };
        _bioText = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill
        };
        bioBox.Controls.Add(_bioText);
        basic.Controls.Add(bioBox, 0, 1);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = false,
            Padding = new Padding(0, 8, 0, 0)
        };
        var saveButton = new Button { Text = "Save Changes", Width = 130, Height = 32 };
        var resetButton = new Button { Text = "Reset Changes", Width = 130, Height = 32 };
        saveButton.Click += async (_, _) => await SaveCurrentCharacterAsync();
        resetButton.Click += (_, _) => ResetCurrentCharacter();
        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(resetButton);
        basic.Controls.Add(buttonPanel, 0, 2);

        BuildAllegianceInfoTab(allegiancePage);
        BuildSkillsTab(skillsPage);
        BuildRealLifeContactTab(realLifePage);
        BuildAwardsTab(awardsPage);
    }


    private void BuildAwardsTab(TabPage page)
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(4, 3, 4, 3),
            ColumnCount = 1,
            RowCount = 2
        };
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        page.Controls.Add(root);

        var awardsBox = new GroupBox
        {
            Text = "Awards",
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };
        root.Controls.Add(awardsBox, 0, 0);

        var awardsLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        awardsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        awardsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        awardsBox.Controls.Add(awardsLayout);

        awardsLayout.Controls.Add(new Label
        {
            Text = "Awards:",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold)
        }, 0, 0);

        _awardsText = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical
        };
        awardsLayout.Controls.Add(_awardsText, 0, 1);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0)
        };
        var saveButton = new Button { Text = "Save Changes", Width = 130, Height = 32 };
        var resetButton = new Button { Text = "Reset Changes", Width = 130, Height = 32 };
        saveButton.Click += async (_, _) => await SaveCurrentCharacterAsync();
        resetButton.Click += (_, _) => ResetCurrentCharacter();
        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(resetButton);
        root.Controls.Add(buttonPanel, 0, 1);
    }


    private void BuildRealLifeContactTab(TabPage page)
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(4, 3, 4, 3),
            ColumnCount = 1,
            RowCount = 3
        };
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        page.Controls.Add(root);

        var realLifeBox = new GroupBox
        {
            Text = "Real Life Info",
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };
        root.Controls.Add(realLifeBox, 0, 0);

        var realGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 4,
            Padding = new Padding(3)
        };
        realGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        realGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        realGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        realGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        realGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        realGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        realLifeBox.Controls.Add(realGrid);

        _realNameText = AddGridTextRow(realGrid, "Real Name:", 0, 1, 1);
        _cityStateText = AddGridTextRow(realGrid, "City, State, Country:", 1, 1, 1);
        AddGridLabel(realGrid, "Misc. Real Life Info:", 2, 0);
        _miscRealLifeText = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Margin = new Padding(0, 2, 0, 2)
        };
        realGrid.Controls.Add(_miscRealLifeText, 0, 3);
        realGrid.SetColumnSpan(_miscRealLifeText, 2);

        var contactBox = new GroupBox
        {
            Text = "Contact Info",
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };
        root.Controls.Add(contactBox, 0, 1);

        var contactGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(3)
        };
        contactGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        contactGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        contactGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        contactGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        contactBox.Controls.Add(contactGrid);

        _emailText = AddGridTextRow(contactGrid, "Email Address:", 0, 1, 1);
        _icqText = AddGridTextRow(contactGrid, "ICQ Number:", 1, 1, 1);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = false,
            Padding = new Padding(0, 8, 0, 0)
        };
        var saveButton = new Button { Text = "Save Changes", Width = 130, Height = 32 };
        var resetButton = new Button { Text = "Reset Changes", Width = 130, Height = 32 };
        saveButton.Click += async (_, _) => await SaveCurrentCharacterAsync();
        resetButton.Click += (_, _) => ResetCurrentCharacter();
        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(resetButton);
        root.Controls.Add(buttonPanel, 0, 2);
    }


    private void BuildSkillsTab(TabPage page)
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(4, 3, 4, 3),
            ColumnCount = 1,
            RowCount = 2
        };
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        page.Controls.Add(root);

        _skillsTabs = new TabControl { Dock = DockStyle.Fill };
        var trainingPage = new TabPage("Training");
        var valuesPage = new TabPage("Skill Values");
        _skillsTabs.TabPages.Add(trainingPage);
        _skillsTabs.TabPages.Add(valuesPage);
        root.Controls.Add(_skillsTabs, 0, 0);

        var trainingBox = new GroupBox
        {
            Text = "Skills",
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };
        trainingPage.Controls.Add(trainingBox);

        var trainingGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 4
        };
        trainingGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        trainingGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 118));
        for (var i = 0; i < 4; i++)
            trainingGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        trainingBox.Controls.Add(trainingGrid);

        _specSkillsList = AddSkillTrainingBlock(trainingGrid, "Specialized Skills", 0, out _lowerSpecButton, lowerText: "Lower", raiseText: null);
        _trainedSkillsList = AddSkillTrainingBlock(trainingGrid, "Trained Skills", 1, out _raiseTrainedButton, out _lowerTrainedButton);
        _untrainedSkillsList = AddSkillTrainingBlock(trainingGrid, "Untrained Skills", 2, out _raiseUntrainedButton, out _lowerUntrainedButton);
        _unusableSkillsList = AddSkillTrainingBlock(trainingGrid, "Unusable Skills", 3, out _raiseUnusableButton, lowerText: null, raiseText: "Raise");

        _lowerSpecButton.Click += (_, _) => MoveSelectedSkills(_specSkillsList, SkillTrainingLevel.Trained);
        _raiseTrainedButton.Click += (_, _) => MoveSelectedSkills(_trainedSkillsList, SkillTrainingLevel.Specialized);
        _lowerTrainedButton.Click += (_, _) => MoveSelectedSkills(_trainedSkillsList, SkillTrainingLevel.Untrained);
        _raiseUntrainedButton.Click += (_, _) => MoveSelectedSkills(_untrainedSkillsList, SkillTrainingLevel.Trained);
        _lowerUntrainedButton.Click += (_, _) => MoveSelectedSkills(_untrainedSkillsList, SkillTrainingLevel.Unusable);
        _raiseUnusableButton.Click += (_, _) => MoveSelectedSkills(_unusableSkillsList, SkillTrainingLevel.Untrained);

        _skillValuesList = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            HideSelection = false,
            LabelEdit = false
        };
        _skillValuesList.Columns.Add("Skill", 260);
        _skillValuesList.Columns.Add("Skill Value", 110);
        _skillValuesList.Columns.Add("Training", 110);
        _skillValuesList.DoubleClick += (_, _) => EditSelectedSkillValue();
        _skillValuesList.Resize += (_, _) => ResizeSkillValueColumns();

        var valuesBox = new GroupBox { Text = "Skills", Dock = DockStyle.Fill, Padding = new Padding(8) };
        valuesBox.Controls.Add(_skillValuesList);
        valuesPage.Controls.Add(valuesBox);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0)
        };
        var saveButton = new Button { Text = "Save Changes", Width = 130, Height = 32 };
        var resetButton = new Button { Text = "Reset Changes", Width = 130, Height = 32 };
        saveButton.Click += async (_, _) => await SaveCurrentCharacterAsync();
        resetButton.Click += (_, _) => ResetCurrentCharacter();
        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(resetButton);
        root.Controls.Add(buttonPanel, 0, 1);
    }

    private static ListBox AddSkillTrainingBlock(TableLayoutPanel parent, string labelText, int row, out Button button, string? lowerText = null, string? raiseText = null)
    {
        var block = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(0, 0, 6, 4) };
        block.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        block.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var label = new Label { Text = labelText, Dock = DockStyle.Fill, Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
        var list = new ListBox { Dock = DockStyle.Fill, SelectionMode = SelectionMode.MultiExtended, Sorted = true };
        block.Controls.Add(label, 0, 0);
        block.Controls.Add(list, 0, 1);
        parent.Controls.Add(block, 0, row);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(4, 26, 0, 0) };
        Button? raise = null;
        Button? lower = null;
        if (!string.IsNullOrWhiteSpace(raiseText))
        {
            raise = new Button { Text = raiseText, Width = 96, Height = 32, Margin = new Padding(0, 0, 0, 8) };
            buttons.Controls.Add(raise);
        }
        if (!string.IsNullOrWhiteSpace(lowerText))
        {
            lower = new Button { Text = lowerText, Width = 96, Height = 32, Margin = new Padding(0, 0, 0, 8) };
            buttons.Controls.Add(lower);
        }
        parent.Controls.Add(buttons, 1, row);
        button = lower ?? raise ?? new Button();
        return list;
    }

    private static ListBox AddSkillTrainingBlock(TableLayoutPanel parent, string labelText, int row, out Button raiseButton, out Button lowerButton)
    {
        var block = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(0, 0, 6, 4) };
        block.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        block.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var label = new Label { Text = labelText, Dock = DockStyle.Fill, Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
        var list = new ListBox { Dock = DockStyle.Fill, SelectionMode = SelectionMode.MultiExtended, Sorted = true };
        block.Controls.Add(label, 0, 0);
        block.Controls.Add(list, 0, 1);
        parent.Controls.Add(block, 0, row);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(4, 26, 0, 0) };
        raiseButton = new Button { Text = "Raise", Width = 96, Height = 32, Margin = new Padding(0, 0, 0, 8) };
        lowerButton = new Button { Text = "Lower", Width = 96, Height = 32, Margin = new Padding(0, 0, 0, 8) };
        buttons.Controls.Add(raiseButton);
        buttons.Controls.Add(lowerButton);
        parent.Controls.Add(buttons, 1, row);
        return list;
    }

    private void DisplaySkills(string rawSkills)
    {
        _currentSkills.Clear();
        _currentSkills.AddRange(LegacySkillParser.Parse(rawSkills));
        PopulateSkillTrainingLists();
        PopulateSkillValuesList();
    }

    private void PopulateSkillTrainingLists()
    {
        if (_specSkillsList == null)
            return;

        _specSkillsList.Items.Clear();
        _trainedSkillsList.Items.Clear();
        _untrainedSkillsList.Items.Clear();
        _unusableSkillsList.Items.Clear();

        foreach (var skill in _currentSkills.OrderBy(s => s.Name))
        {
            var target = skill.TrainingLevel switch
            {
                SkillTrainingLevel.Specialized => _specSkillsList,
                SkillTrainingLevel.Trained => _trainedSkillsList,
                SkillTrainingLevel.Untrained => _untrainedSkillsList,
                _ => _unusableSkillsList
            };
            target.Items.Add(skill.Name);
        }
    }

    private void PopulateSkillValuesList()
    {
        if (_skillValuesList == null)
            return;

        _skillValuesList.BeginUpdate();
        _skillValuesList.Items.Clear();
        foreach (var group in GetGroupedSkillsForValueDisplay())
        {
            var header = new ListViewItem(LegacySkillParser.TrainingLabel(group.Key))
            {
                Font = new Font(_skillValuesList.Font, FontStyle.Bold),
                Tag = null
            };
            header.SubItems.Add(string.Empty);
            header.SubItems.Add(string.Empty);
            _skillValuesList.Items.Add(header);

            foreach (var skill in OrderSkillsWithinTrainingGroup(group))
            {
                var item = new ListViewItem("  " + skill.Name) { Tag = skill };
                item.SubItems.Add(skill.HasValue ? skill.Value.ToString() : string.Empty);
                item.SubItems.Add(LegacySkillParser.TrainingLabel(skill.TrainingLevel));
                _skillValuesList.Items.Add(item);
            }
        }
        ResizeSkillValueColumns();
        _skillValuesList.EndUpdate();
    }


    private IEnumerable<IGrouping<SkillTrainingLevel, SkillInfo>> GetGroupedSkillsForValueDisplay()
    {
        return _currentSkills
            .GroupBy(s => s.TrainingLevel)
            .OrderBy(g => TrainingSortOrder(g.Key));
    }

    private static IEnumerable<SkillInfo> OrderSkillsWithinTrainingGroup(IEnumerable<SkillInfo> skills)
    {
        return skills
            .OrderByDescending(s => s.HasValue)
            .ThenBy(s => s.LegacyOrder);
    }

    private static int TrainingSortOrder(SkillTrainingLevel level) => level switch
    {
        SkillTrainingLevel.Specialized => 0,
        SkillTrainingLevel.Trained => 1,
        SkillTrainingLevel.Untrained => 2,
        SkillTrainingLevel.Unusable => 3,
        _ => 4
    };

    private void ResizeSkillValueColumns()
    {
        if (_skillValuesList.Columns.Count < 3)
            return;
        var width = Math.Max(360, _skillValuesList.ClientSize.Width - 4);
        _skillValuesList.Columns[0].Width = Math.Max(180, width - 224);
        _skillValuesList.Columns[1].Width = 104;
        _skillValuesList.Columns[2].Width = 116;
    }

    private void MoveSelectedSkills(ListBox source, SkillTrainingLevel newLevel)
    {
        if (source.SelectedItems.Count == 0)
            return;

        var selected = source.SelectedItems.Cast<string>().ToList();
        foreach (var skillName in selected)
        {
            var skill = _currentSkills.FirstOrDefault(s => string.Equals(s.Name, skillName, StringComparison.OrdinalIgnoreCase));
            if (skill != null)
                skill.TrainingLevel = newLevel;
        }
        PopulateSkillTrainingLists();
        PopulateSkillValuesList();
        _statusMain.Text = "Skill training changed";
        _statusConnection.Text = "Click Save Changes to upload";
    }

    private void EditSelectedSkillValue()
    {
        if (_skillValuesList.SelectedItems.Count == 0 || _skillValuesList.SelectedItems[0].Tag is not SkillInfo skill)
            return;

        var value = PromptForText(this, "Skill Value", $"Enter the skill value for {skill.Name}:");
        if (value == null)
            return;
        if (!int.TryParse(value.Trim(), out var parsed))
        {
            MessageBox.Show(this, "Skill value must be a number.", "ACGM Modern", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        skill.Value = parsed;
        PopulateSkillValuesList();
        _statusMain.Text = "Skill value changed";
        _statusConnection.Text = "Click Save Changes to upload";
    }

    private string BuildSkillsFromControls()
    {
        if (_currentSkills.Count == 0)
            return _currentDetails?.Skills ?? string.Empty;
        return LegacySkillParser.Serialize(_currentSkills);
    }


    private void BuildAllegianceInfoTab(TabPage page)
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(4, 3, 4, 3),
            ColumnCount = 1,
            RowCount = 3
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 178));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 132));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        page.Controls.Add(root);

        var summaryBox = new GroupBox
        {
            Text = "Allegiance Info",
            Dock = DockStyle.Fill,
            Padding = new Padding(7)
        };
        root.Controls.Add(summaryBox, 0, 0);

        var summaryGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 6,
            Padding = new Padding(3)
        };
        summaryGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 118));
        summaryGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        summaryGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 128));
        summaryGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        for (var i = 0; i < 6; i++)
            summaryGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
        summaryBox.Controls.Add(summaryGrid);

        _allegianceCharacterText = AddReadOnlyGridTextRow(summaryGrid, "Character:", 0, 1, 3);
        _allegiancePatronText = AddReadOnlyGridTextRow(summaryGrid, "Patron:", 1, 1, 2);
        _changePatronButton = new Button { Text = "Change Patron", Dock = DockStyle.Fill, Margin = new Padding(4, 1, 4, 1) };
        _changePatronButton.Click += (_, _) => ChangePatronWorkflow();
        summaryGrid.Controls.Add(_changePatronButton, 3, 1);
        _allegianceMonarchText = AddReadOnlyGridTextRow(summaryGrid, "Monarch:", 2, 1, 3);
        AddGridLabel(summaryGrid, "Number of Followers:", 3, 0);
        _allegianceDirectCountText = CreateReadOnlyTextBox();
        summaryGrid.Controls.Add(_allegianceDirectCountText, 1, 3);
        AddGridLabel(summaryGrid, "Followers in Tree:", 3, 2);
        _allegianceTotalCountText = CreateReadOnlyTextBox();
        summaryGrid.Controls.Add(_allegianceTotalCountText, 3, 3);
        _allegiancePathText = AddReadOnlyGridTextRow(summaryGrid, "Path to Monarch:", 4, 1, 3);

        var allegianceButtons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 3, 0, 0),
            Margin = new Padding(0)
        };
        _allegianceSaveButton = new Button { Text = "Save Changes", Width = 130, Height = 30 };
        _allegianceResetButton = new Button { Text = "Reset Changes", Width = 130, Height = 30 };
        _allegianceSaveButton.Click += async (_, _) => await SaveAllegianceChangesAsync();
        _allegianceResetButton.Click += (_, _) => ResetAllegianceChanges();
        allegianceButtons.Controls.Add(_allegianceSaveButton);
        allegianceButtons.Controls.Add(_allegianceResetButton);
        summaryGrid.Controls.Add(allegianceButtons, 1, 5);
        summaryGrid.SetColumnSpan(allegianceButtons, 3);

        var vassalsBox = new GroupBox
        {
            Text = "Vassals",
            Dock = DockStyle.Fill,
            Padding = new Padding(7)
        };
        root.Controls.Add(vassalsBox, 0, 1);

        var vassalLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        vassalLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        vassalLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 132));
        vassalsBox.Controls.Add(vassalLayout);

        _directVassalsList = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            HideSelection = false,
            HeaderStyle = ColumnHeaderStyle.None,
            MultiSelect = false
        };
        _directVassalsList.Columns.Add(string.Empty, Math.Max(100, _directVassalsList.ClientSize.Width - 4));
        _directVassalsList.Resize += (_, _) => ResizeDirectVassalsColumn();
        _directVassalsList.DoubleClick += async (_, _) => await SelectDirectVassalAsync();
        vassalLayout.Controls.Add(_directVassalsList, 0, 0);

        var vassalButtons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(6, 0, 0, 0)
        };
        _addVassalButton = new Button { Text = "Add Vassal", Width = 116, Height = 36 };
        _removeVassalButton = new Button { Text = "Remove Vassal", Width = 116, Height = 36 };
        _addVassalButton.Click += (_, _) => AddVassalWorkflow();
        _removeVassalButton.Click += (_, _) => RemoveVassalWorkflow();
        vassalButtons.Controls.Add(_addVassalButton);
        vassalButtons.Controls.Add(_removeVassalButton);
        vassalLayout.Controls.Add(vassalButtons, 1, 0);

        var immediateBox = new GroupBox
        {
            Text = "Immediate Tree",
            Dock = DockStyle.Fill,
            Padding = new Padding(7)
        };
        root.Controls.Add(immediateBox, 0, 2);

        var immediateLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        immediateLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        immediateLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        immediateBox.Controls.Add(immediateLayout);

        _immediateTreeView = new TreeView
        {
            Dock = DockStyle.Fill,
            HideSelection = false,
            ImageList = _treeImages,
            BorderStyle = BorderStyle.Fixed3D
        };
        _immediateTreeView.DoubleClick += async (_, _) => await SelectImmediateTreeNodeAsync();
        immediateLayout.Controls.Add(_immediateTreeView, 0, 0);

        _allegianceNotesText = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill
        };
        immediateLayout.Controls.Add(_allegianceNotesText, 0, 1);
    }

    private static TextBox CreateReadOnlyTextBox() => new()
    {
        Dock = DockStyle.Fill,
        Margin = new Padding(0, 2, 4, 2),
        ReadOnly = true,
        BackColor = SystemColors.Window
    };

    private static TextBox AddReadOnlyGridTextRow(TableLayoutPanel parent, string label, int row, int fieldColumn, int fieldSpan)
    {
        AddGridLabel(parent, label, row, 0);
        var box = CreateReadOnlyTextBox();
        parent.Controls.Add(box, fieldColumn, row);
        parent.SetColumnSpan(box, fieldSpan);
        return box;
    }

    private async Task SelectDirectVassalAsync()
    {
        if (_directVassalsList.SelectedItems.Count == 0)
            return;
        if (_directVassalsList.SelectedItems[0].Tag is not CharacterRecord record)
            return;

        var node = FindNodeByName(_treeView.Nodes, record.Name);
        if (node == null)
            return;

        _treeView.SelectedNode = node;
        node.EnsureVisible();
        await LoadSelectedCharacterAsync();
    }

    private static TextBox CreateTextBox() => new()
    {
        Dock = DockStyle.Fill,
        Margin = new Padding(0, 2, 4, 2)
    };

    private static ComboBox CreateComboBox() => new()
    {
        Dock = DockStyle.Fill,
        DropDownStyle = ComboBoxStyle.DropDown,
        Margin = new Padding(0, 2, 4, 2)
    };

    private static Label CreateGridLabel(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleLeft,
        Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold),
        AutoEllipsis = true,
        Margin = new Padding(0, 0, 8, 0)
    };

    private static void AddGridLabel(TableLayoutPanel parent, string text, int row, int column)
    {
        parent.Controls.Add(CreateGridLabel(text), column, row);
    }

    private static TextBox AddGridTextRow(TableLayoutPanel parent, string label, int row, int fieldColumn, int fieldSpan)
    {
        AddGridLabel(parent, label, row, 0);
        var box = CreateTextBox();
        parent.Controls.Add(box, fieldColumn, row);
        parent.SetColumnSpan(box, fieldSpan);
        return box;
    }

    private static ComboBox AddGridComboRow(TableLayoutPanel parent, string label, int row, int fieldColumn, int fieldSpan)
    {
        AddGridLabel(parent, label, row, 0);
        var box = CreateComboBox();
        parent.Controls.Add(box, fieldColumn, row);
        parent.SetColumnSpan(box, fieldSpan);
        return box;
    }


    private async Task RefreshRescueSquadAsync()
    {
        _statusMain.Text = "Downloading Rescue Squad Info";
        _statusConnection.Text = "Connecting to Server";
        Cursor = Cursors.WaitCursor;
        try
        {
            var result = await _protocol.GetRescueSquadAsync();
            PopulateRescueSquadList(result.Entries);
            _statusBytes.Text = $"{result.Bytes} bytes received";
            _statusConnection.Text = "Closing Connection";
            _statusMain.Text = "Rescue Squad loaded";
        }
        catch (Exception ex)
        {
            ShowUtilityLoadError("Rescue Squad load failed", ex);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private async Task RefreshPortalListAsync()
    {
        _statusMain.Text = "Downloading Summonable Portal Info";
        _statusConnection.Text = "Connecting to Server";
        Cursor = Cursors.WaitCursor;
        try
        {
            var result = await _protocol.GetPortalListAsync();
            PopulatePortalList(result.Entries);
            _statusBytes.Text = $"{result.Bytes} bytes received";
            _statusConnection.Text = "Closing Connection";
            _statusMain.Text = "Portal list loaded";
        }
        catch (Exception ex)
        {
            ShowUtilityLoadError("Portal list load failed", ex);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private async Task RefreshTradeSkillsListAsync()
    {
        _statusMain.Text = "Downloading Trade Skill Info";
        _statusConnection.Text = "Connecting to Server";
        Cursor = Cursors.WaitCursor;
        try
        {
            var result = await _protocol.GetTradeSkillListAsync();
            PopulateTradeSkillsList(result.Entries);
            _statusBytes.Text = $"{result.Bytes} bytes received";
            _statusConnection.Text = "Closing Connection";
            _statusMain.Text = "Trade skills loaded";
        }
        catch (Exception ex)
        {
            ShowUtilityLoadError("Trade skills load failed", ex);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void ShowUtilityLoadError(string message, Exception ex)
    {
        _statusMain.Text = message;
        _statusConnection.Text = ex.Message;
        AcgmLogger.TryWrite(ex.ToString());
        MessageBox.Show(this, ex.ToString(), "ACGM Modern", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void PopulateRescueSquadList(IEnumerable<RescueSquadEntry> entries)
    {
        _rescueSquadList.Items.Clear();
        foreach (var entry in entries)
        {
            var item = new ListViewItem(entry.Name) { Tag = entry.Name };
            item.SubItems.Add(entry.Level);
            item.SubItems.Add(entry.Lifestone);
            item.SubItems.Add(entry.TiedTo);
            item.SubItems.Add(entry.CanSummon);
            item.SubItems.Add(entry.MainCharacterName);
            item.SubItems.Add(entry.EmailAddress);
            item.SubItems.Add(entry.IcqNumber);
            _rescueSquadList.Items.Add(item);
        }
    }

    private void PopulatePortalList(IEnumerable<PortalListEntry> entries)
    {
        _portalList.Items.Clear();
        foreach (var entry in entries)
        {
            var item = new ListViewItem(entry.Name) { Tag = entry.Name };
            item.SubItems.Add(entry.Level);
            item.SubItems.Add(entry.TiedTo);
            item.SubItems.Add(entry.Lifestone);
            _portalList.Items.Add(item);
        }
    }

    private void PopulateTradeSkillsList(IEnumerable<TradeSkillEntry> entries)
    {
        _tradeSkillsList.Items.Clear();
        foreach (var entry in entries)
        {
            var item = new ListViewItem(entry.Name) { Tag = entry.Name };
            item.SubItems.Add(entry.Level);
            item.SubItems.Add(entry.Alchemy);
            item.SubItems.Add(entry.Cooking);
            item.SubItems.Add(entry.Fletching);
            _tradeSkillsList.Items.Add(item);
        }
    }

    private async Task SelectUtilityCharacterAsync(ListView list)
    {
        if (list.SelectedItems.Count == 0 || list.SelectedItems[0].Tag is not string characterName || string.IsNullOrWhiteSpace(characterName))
            return;

        var node = FindNodeByName(_treeView.Nodes, characterName);
        if (node == null)
            return;

        _treeView.SelectedNode = node;
        node.EnsureVisible();
        _mainTabs.SelectedIndex = 0;
        await LoadSelectedCharacterAsync();
    }

    private async Task RefreshTreeAsync()
    {
        _statusMain.Text = "Downloading Tree Info";
        _statusConnection.Text = "Connecting to Server";
        _statusBytes.Text = string.Empty;
        Cursor = Cursors.WaitCursor;

        try
        {
            var treeResult = await _protocol.GetTreeAsync();
            _statusBytes.Text = $"{treeResult.Bytes} bytes received";
            _statusConnection.Text = "Closing Connection";

            var records = treeResult.Records;
            CalculateComputedRanks(records);
            _charactersById.Clear();
            foreach (var record in records)
                _charactersById[record.Id] = record;

            _searchDetailsCache.Clear();
            _searchResultsList?.Items.Clear();

            PopulateTree(records);
            PopulateFlatList(records);
            _statusMain.Text = "Tree loaded";
        }
        catch (Exception ex)
        {
            _statusMain.Text = "Error";
            _statusConnection.Text = ex.Message;
            AcgmLogger.TryWrite(ex.ToString());
            MessageBox.Show(this, ex.ToString(), "ACGM Modern", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private static void CalculateComputedRanks(List<CharacterRecord> records)
    {
        var byId = records.ToDictionary(r => r.Id);
        var visiting = new HashSet<int>();

        int Calculate(CharacterRecord record)
        {
            if (record.ComputedRank > 0)
                return record.ComputedRank;
            if (!visiting.Add(record.Id))
                return Math.Max(1, record.Rank);

            var vassalIds = record.VassalIds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(id => int.TryParse(id, out var parsed) ? parsed : 0)
                .Where(id => id > 0 && byId.ContainsKey(id))
                .Select(id => byId[id])
                .ToList();

            var computed = 1;
            if (vassalIds.Count >= 2)
            {
                var topRanks = vassalIds.Select(Calculate).OrderByDescending(rank => rank).Take(2).ToList();
                computed = topRanks.Count >= 2 && topRanks[0] == topRanks[1]
                    ? Math.Min(10, topRanks[0] + 1)
                    : topRanks[0];
            }

            record.ComputedRank = computed;
            visiting.Remove(record.Id);
            return computed;
        }

        foreach (var record in records)
            Calculate(record);
    }

    private void PopulateTree(List<CharacterRecord> records)
    {
        _treeView.BeginUpdate();
        _treeView.Nodes.Clear();

        var childrenByPatron = records.GroupBy(r => r.PatronId).ToDictionary(g => g.Key, g => g.OrderBy(r => r.Name).ToList());
        var roots = childrenByPatron.TryGetValue(-1, out var rootList) ? rootList : records.Where(r => r.PatronId <= 0).OrderBy(r => r.Name).ToList();

        foreach (var root in roots)
            _treeView.Nodes.Add(BuildNode(root, childrenByPatron));

        if (_treeView.Nodes.Count > 0)
        {
            _treeView.Nodes[0].Expand();
            var preferred = !string.IsNullOrWhiteSpace(_settings.LastSelectedCharacter)
                ? _settings.LastSelectedCharacter
                : _characterName;
            _treeView.SelectedNode = FindNodeByName(_treeView.Nodes, preferred)
                ?? FindNodeByName(_treeView.Nodes, _characterName)
                ?? _treeView.Nodes[0];
            _treeView.SelectedNode?.EnsureVisible();
        }

        _treeView.EndUpdate();
    }

    private TreeNode BuildNode(CharacterRecord record, Dictionary<int, List<CharacterRecord>> childrenByPatron)
    {
        var imageKey = GetTreeImageKey(record);
        var node = new TreeNode(record.DisplayText) { Name = $"Char{record.Id}", Tag = record, ImageKey = imageKey, SelectedImageKey = imageKey };
        if (childrenByPatron.TryGetValue(record.Id, out var children))
        {
            foreach (var child in children)
                node.Nodes.Add(BuildNode(child, childrenByPatron));
        }
        return node;
    }

    private static TreeNode? FindNodeByName(TreeNodeCollection nodes, string characterName)
    {
        foreach (TreeNode node in nodes)
        {
            if (node.Tag is CharacterRecord record && string.Equals(record.Name, characterName, StringComparison.OrdinalIgnoreCase))
                return node;
            var child = FindNodeByName(node.Nodes, characterName);
            if (child != null)
                return child;
        }
        return null;
    }

    private void PopulateFlatList(IEnumerable<CharacterRecord> records)
    {
        _flatList.Items.Clear();
        foreach (var record in records.OrderBy(r => r.Name))
        {
            var item = new ListViewItem(record.Name) { Tag = record };
            item.SubItems.Add(record.Level.ToString());
            item.SubItems.Add(record.PatronName);
            _flatList.Items.Add(item);
        }
    }

    private async Task LoadSelectedCharacterAsync()
    {
        var record = SelectedCharacter;
        if (record == null)
            return;

        DisplayTreeRecord(record);
        _settings.LastSelectedCharacter = record.Name;
        _settings.Save();
        _statusMain.Text = "Downloading Character Info";
        _statusConnection.Text = "Connecting to Server";

        try
        {
            var detailResult = await _protocol.GetCharacterInfoAsync(record.Id);
            _statusBytes.Text = $"{detailResult.Bytes} bytes received";
            DisplayCharacterDetails(detailResult.Details, record);
        }
        catch (Exception ex)
        {
            _statusConnection.Text = "Character detail unavailable";
            _lastModifiedLabel.Text = "Last Modified: detail request failed";
            _bioText.Text = ex.Message;
            AcgmLogger.TryWrite(ex.ToString());
        }
    }

    private void DisplayTreeRecord(CharacterRecord record)
    {
        _nameText.Text = record.Name;
        _levelText.Text = record.Level.ToString();
        _classText.Text = string.Empty;
        PopulateChoiceCombo(_raceCombo, LegacyCharacterParser.RaceChoices, record.Race.ToString());
        PopulateChoiceCombo(_sexCombo, LegacyCharacterParser.SexChoices, string.Empty);
        PopulateRankCombo(_rankCombo, record.Race.ToString(), string.Empty, record.Rank.ToString());
        PopulateRankCombo(_computedRankCombo, record.Race.ToString(), string.Empty, record.ComputedRank.ToString());
        _muleYes.Checked = record.IsMule;
        _muleNo.Checked = !record.IsMule;
        _muleForText.Text = string.Empty;
        _mainCharacterText.Text = record.Name;
        _lifestoneText.Text = string.Empty;
        _tiedToText.Text = string.Empty;
        _canSummonCheck.Checked = false;
        _pkCheck.Checked = record.IsPk;
        _rescueCheck.Checked = record.IsRescueSquad;
        _hideInfoCheck.Checked = false;
        _lastModifiedLabel.Text = "Last Modified:";
        _bioText.Text = string.Empty;
        ClearRealLifeContactInfo();
        ClearAwardsInfo();
        DisplaySkills(string.Empty);
        ClearAllegianceInfo(record);
    }


    private void ClearRealLifeContactInfo()
    {
        if (_realNameText == null)
            return;
        _realNameText.Text = string.Empty;
        _cityStateText.Text = string.Empty;
        _miscRealLifeText.Text = string.Empty;
        _emailText.Text = string.Empty;
        _icqText.Text = string.Empty;
    }


    private void ClearAwardsInfo()
    {
        if (_awardsText == null)
            return;
        _awardsText.Text = string.Empty;
    }


    private void ClearAllegianceInfo(CharacterRecord record)
    {
        if (_allegianceCharacterText == null)
            return;
        _allegianceCharacterText.Text = record.Name;
        _allegiancePatronText.Text = record.PatronName;
        _allegianceMonarchText.Text = string.Empty;
        _allegianceDirectCountText.Text = string.Empty;
        _allegianceTotalCountText.Text = string.Empty;
        _allegiancePathText.Text = string.Empty;
        _directVassalsList.Items.Clear();
        _allegianceNotesText.Text = "Allegiance details load with the character detail response.";
    }

    private void DisplayCharacterDetails(CharacterDetails details, CharacterRecord fallback)
    {
        var name = FirstNonEmpty(details.Name, fallback.Name);
        var level = FirstNonEmpty(details.Level, fallback.Level.ToString());
        var className = details.ClassName;
        var raceValue = FirstNonEmpty(details.Race, fallback.Race.ToString());
        var sexValue = details.Sex;
        var rankValue = FirstNonEmpty(details.Rank, fallback.Rank.ToString());
        var computedRankValue = FirstNonEmpty(details.ComputedRank, fallback.ComputedRank.ToString());

        _nameText.Text = name;
        _levelText.Text = level;
        _classText.Text = className;

        PopulateChoiceCombo(_raceCombo, LegacyCharacterParser.RaceChoices, raceValue);
        PopulateChoiceCombo(_sexCombo, LegacyCharacterParser.SexChoices, sexValue);
        PopulateRankCombo(_rankCombo, raceValue, sexValue, rankValue);
        PopulateRankCombo(_computedRankCombo, raceValue, sexValue, computedRankValue);

        _muleYes.Checked = details.IsMule;
        _muleNo.Checked = !details.IsMule;
        _muleForText.Text = details.MuleFor;
        _mainCharacterText.Text = FirstNonEmpty(details.MainCharacterName, name);
        _lifestoneText.Text = details.LifestonedAt;
        _tiedToText.Text = details.TiedTo;
        _canSummonCheck.Checked = details.CanSummon;
        _pkCheck.Checked = details.IsPk;
        _rescueCheck.Checked = details.IsRescueMember;
        _hideInfoCheck.Checked = details.HideInfoOnWeb;

        var modifiedBy = details.LastModifiedByCharacterId > 0 && _charactersById.TryGetValue(details.LastModifiedByCharacterId, out var editor)
            ? editor.Name
            : details.LastModifiedBy;
        _lastModifiedLabel.Text = string.IsNullOrWhiteSpace(details.LastModified)
            ? "Last Modified:"
            : string.IsNullOrWhiteSpace(modifiedBy)
                ? $"Last Modified: {details.LastModified}"
                : $"Last Modified: {details.LastModified} by {modifiedBy}";

        _bioText.Text = details.Bio;
        DisplayRealLifeContactInfo(details);
        DisplayAwardsInfo(details);
        DisplaySkills(details.Skills);
        _currentDetails = details;
        _currentRecord = fallback;
        DisplayAllegianceInfo(details, fallback);
        _statusMain.Text = "Character info loaded";
        _statusConnection.Text = "Closing Connection";
    }


    private void DisplayAwardsInfo(CharacterDetails details)
    {
        if (_awardsText == null)
            return;
        _awardsText.Text = details.Awards;
    }


    private void DisplayRealLifeContactInfo(CharacterDetails details)
    {
        if (_realNameText == null)
            return;
        _realNameText.Text = details.RealName;
        _cityStateText.Text = details.CityState;
        _miscRealLifeText.Text = details.MiscRealLife;
        _emailText.Text = details.Email;
        _icqText.Text = details.Icq;
    }


    private void DisplayAllegianceInfo(CharacterDetails details, CharacterRecord fallback)
    {
        var allegiance = AllegianceInfoBuilder.Build(details, fallback, _charactersById);
        _currentAllegianceInfo = allegiance;
        _allegianceCharacter = fallback;
        _pendingAddedVassals.Clear();
        _pendingRemovedVassals.Clear();
        _pendingNewPatron = null;

        _allegianceCharacterText.Text = allegiance.CharacterName;
        _allegiancePatronText.Text = string.IsNullOrWhiteSpace(allegiance.Patron) ? "None / Monarch" : allegiance.Patron;
        _allegianceMonarchText.Text = allegiance.Monarch;
        _allegianceDirectCountText.Text = !string.IsNullOrWhiteSpace(details.FollowerCount) ? details.FollowerCount : allegiance.DirectVassalCount.ToString();
        _allegianceTotalCountText.Text = allegiance.TotalKnownVassalCount.ToString();
        _allegiancePathText.Text = string.IsNullOrWhiteSpace(allegiance.PathToMonarch)
            ? BuildPathFromTree(fallback)
            : allegiance.PathToMonarch;

        PopulateAllegianceVassals(allegiance.DirectVassals);
        PopulateImmediateTree(fallback);
        UpdateAllegianceNotes(allegiance.Notes + Environment.NewLine + allegiance.SourceSummary);
    }

    private void PopulateAllegianceVassals(IEnumerable<CharacterRecord> vassals)
    {
        _directVassalsList.BeginUpdate();
        _directVassalsList.Items.Clear();
        foreach (var vassal in vassals.OrderBy(v => v.Name))
        {
            var item = new ListViewItem(vassal.DisplayText) { Tag = vassal, ImageKey = GetTreeImageKey(vassal) };
            _directVassalsList.Items.Add(item);
        }
        ResizeDirectVassalsColumn();
        _directVassalsList.EndUpdate();
    }

    private void ResizeDirectVassalsColumn()
    {
        if (_directVassalsList.Columns.Count == 0)
            return;

        _directVassalsList.Columns[0].Width = Math.Max(100, _directVassalsList.ClientSize.Width - 4);
    }

    private void PopulateImmediateTree(CharacterRecord rootRecord)
    {
        _immediateTreeView.BeginUpdate();
        _immediateTreeView.Nodes.Clear();
        var childrenByPatron = _charactersById.Values.GroupBy(r => r.PatronId).ToDictionary(g => g.Key, g => g.OrderBy(r => r.Name).ToList());
        _immediateTreeView.Nodes.Add(BuildNode(rootRecord, childrenByPatron));
        if (_immediateTreeView.Nodes.Count > 0)
            _immediateTreeView.Nodes[0].Expand();
        _immediateTreeView.EndUpdate();
    }

    private void UpdateAllegianceNotes(string baseNotes)
    {
        var lines = new List<string>();
        if (!string.IsNullOrWhiteSpace(baseNotes))
            lines.Add(baseNotes.Trim());
        if (_pendingNewPatron != null)
            lines.Add($"Pending patron change: {_pendingNewPatron.Name}");
        if (_pendingAddedVassals.Count > 0)
            lines.Add("Pending vassal adds: " + string.Join(", ", _pendingAddedVassals));
        if (_pendingRemovedVassals.Count > 0)
            lines.Add("Pending vassal removals: " + string.Join(", ", _pendingRemovedVassals.Select(v => v.Name)));
        _allegianceNotesText.Text = string.Join(Environment.NewLine, lines);
    }

    private async Task SelectImmediateTreeNodeAsync()
    {
        if (_immediateTreeView.SelectedNode?.Tag is not CharacterRecord record)
            return;
        var node = FindNodeByName(_treeView.Nodes, record.Name);
        if (node == null)
            return;
        _treeView.SelectedNode = node;
        node.EnsureVisible();
        await LoadSelectedCharacterAsync();
    }

    private void AddVassalWorkflow()
    {
        if (_allegianceCharacter == null)
            return;

        var name = PromptForText(this, "Add Vassal", "Enter the vassal character name:");
        if (string.IsNullOrWhiteSpace(name))
            return;

        name = name.Trim();
        if (_directVassalsList.Items.Cast<ListViewItem>().Any(i => string.Equals(i.Text, name, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show(this, "That character is already listed as a direct vassal.", "ACGM Modern", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _pendingAddedVassals.Add(name);
        var item = new ListViewItem(name + " - pending add") { ForeColor = SystemColors.GrayText };
        _directVassalsList.Items.Add(item);
        _statusMain.Text = "Vassal add pending";
        _statusConnection.Text = "Click Save Changes to upload";
        UpdateAllegianceNotes(_currentAllegianceInfo?.Notes + Environment.NewLine + _currentAllegianceInfo?.SourceSummary);
    }

    private void RemoveVassalWorkflow()
    {
        if (_allegianceCharacter == null || _directVassalsList.SelectedItems.Count == 0)
            return;

        var item = _directVassalsList.SelectedItems[0];
        if (item.Tag is not CharacterRecord record)
        {
            _directVassalsList.Items.Remove(item);
            _pendingAddedVassals.RemoveAll(v => string.Equals(v, item.Text.Replace(" - pending add", string.Empty), StringComparison.OrdinalIgnoreCase));
            return;
        }

        var response = MessageBox.Show(this,
            $"Are you sure you want to remove your vassal {record.Name}?\nThis will also remove all characters that are followers of {record.Name}.",
            "Remove Vassal Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (response != DialogResult.Yes)
            return;

        if (!_pendingRemovedVassals.Any(v => v.Id == record.Id))
            _pendingRemovedVassals.Add(record);
        item.ForeColor = SystemColors.GrayText;
        item.Text = record.DisplayText + " - pending remove";
        _statusMain.Text = "Vassal removal pending";
        _statusConnection.Text = "Click Save Changes to upload";
        UpdateAllegianceNotes(_currentAllegianceInfo?.Notes + Environment.NewLine + _currentAllegianceInfo?.SourceSummary);
    }

    private void ChangePatronWorkflow()
    {
        if (_allegianceCharacter == null)
            return;

        var name = PromptForText(this, "Change Patron", "Enter the new patron character name:");
        if (string.IsNullOrWhiteSpace(name))
            return;

        var newPatron = _charactersById.Values.FirstOrDefault(r => string.Equals(r.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
        if (newPatron == null)
        {
            MessageBox.Show(this, "That character was not found in the downloaded tree. Refresh the tree and try again.", "ACGM Modern", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (newPatron.Id == _allegianceCharacter.Id)
        {
            MessageBox.Show(this, "A character cannot pledge to itself.", "ACGM Modern", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _pendingNewPatron = newPatron;
        _allegiancePatronText.Text = newPatron.Name + " (pending)";
        _statusMain.Text = "Patron change pending";
        _statusConnection.Text = "Click Save Changes to upload";
        UpdateAllegianceNotes(_currentAllegianceInfo?.Notes + Environment.NewLine + _currentAllegianceInfo?.SourceSummary);
    }

    private async Task CurrentPlayerAddVassalAsync()
    {
        var targetPatron = SelectedCharacter;
        if (targetPatron == null)
        {
            MessageBox.Show(this, "Select the character who will receive the new vassal.", "Add Vassal", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var loginRecord = GetLoggedInCharacterRecord();
        if (loginRecord == null)
        {
            MessageBox.Show(this, "Could not find the logged-in character in the downloaded tree. Refresh the tree and try again.", "Add Vassal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var vassalName = PromptForText(this, "Add Vassal", "Enter the vassal character name:");
        if (string.IsNullOrWhiteSpace(vassalName))
            return;

        _statusMain.Text = "Adding Vassal";
        _statusConnection.Text = "Connecting to Server";
        Cursor = Cursors.WaitCursor;
        try
        {
            var bytes = await _protocol.AddVassalAsync(loginRecord.Id, targetPatron, vassalName.Trim());
            _statusBytes.Text = $"{bytes} bytes received";
            _statusConnection.Text = "Closing Connection";
            _statusMain.Text = "Vassal added";
            await RefreshTreeAsync();
        }
        catch (Exception ex)
        {
            _statusMain.Text = "Add Vassal failed";
            _statusConnection.Text = ex.Message;
            AcgmLogger.TryWrite(ex.ToString());
            MessageBox.Show(this, ex.ToString(), "ACGM Modern Add Vassal Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private async Task CurrentPlayerChangePatronAsync()
    {
        var movingCharacter = SelectedCharacter;
        if (movingCharacter == null)
        {
            MessageBox.Show(this, "Select the character whose patron should be changed.", "Change Patron", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var loginRecord = GetLoggedInCharacterRecord();
        if (loginRecord == null)
        {
            MessageBox.Show(this, "Could not find the logged-in character in the downloaded tree. Refresh the tree and try again.", "Change Patron", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var patronName = PromptForText(this, "Change Patron", "Enter the new patron character name:");
        if (string.IsNullOrWhiteSpace(patronName))
            return;

        var newPatron = _charactersById.Values.FirstOrDefault(r => string.Equals(r.Name, patronName.Trim(), StringComparison.OrdinalIgnoreCase));
        if (newPatron == null)
        {
            MessageBox.Show(this, "That character was not found in the downloaded tree. Refresh the tree and try again.", "Change Patron", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (newPatron.Id == movingCharacter.Id)
        {
            MessageBox.Show(this, "A character cannot pledge to itself.", "Change Patron", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var oldPatron = movingCharacter.PatronId > 0 && _charactersById.TryGetValue(movingCharacter.PatronId, out var resolvedOldPatron)
            ? resolvedOldPatron
            : null;

        _statusMain.Text = "Changing Patrons";
        _statusConnection.Text = "Connecting to Server";
        Cursor = Cursors.WaitCursor;
        try
        {
            var bytes = await _protocol.ChangePatronAsync(loginRecord.Id, movingCharacter, oldPatron, newPatron);
            _statusBytes.Text = $"{bytes} bytes received";
            _statusConnection.Text = "Closing Connection";
            _statusMain.Text = "Patron changed";
            await RefreshTreeAsync();
        }
        catch (Exception ex)
        {
            _statusMain.Text = "Change Patron failed";
            _statusConnection.Text = ex.Message;
            AcgmLogger.TryWrite(ex.ToString());
            MessageBox.Show(this, ex.ToString(), "ACGM Modern Change Patron Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private async Task CurrentPlayerChangePasswordAsync()
    {
        var target = SelectedCharacter;
        if (target == null)
        {
            MessageBox.Show(this, "Select the character whose password should be changed.", "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var newPassword = PromptForPasswordChange(this);
        if (string.IsNullOrWhiteSpace(newPassword))
            return;

        _statusMain.Text = "Changing Password";
        _statusConnection.Text = "Connecting to Server";
        Cursor = Cursors.WaitCursor;
        try
        {
            var bytes = await _protocol.ChangePasswordAsync(target.Id, newPassword);
            _statusBytes.Text = $"{bytes} bytes received";
            _statusConnection.Text = "Closing Connection";
            _statusMain.Text = "Password changed";

            if (string.Equals(target.Name, _characterName, StringComparison.OrdinalIgnoreCase))
            {
                _password = newPassword;
                _protocol = new AcgmProtocolService(_endpoint, _characterName, _password);
            }

            MessageBox.Show(this, "The character password has been changed.", "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _statusMain.Text = "Change Password failed";
            _statusConnection.Text = ex.Message;
            AcgmLogger.TryWrite(ex.ToString());
            MessageBox.Show(this, ex.ToString(), "ACGM Modern Change Password Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void ResetAllegianceChanges()
    {
        if (_currentDetails == null || _currentRecord == null)
            return;
        DisplayAllegianceInfo(_currentDetails, _currentRecord);
        _statusMain.Text = "Allegiance changes reset";
        _statusConnection.Text = "Ready";
    }

    private async Task SaveAllegianceChangesAsync()
    {
        if (_allegianceCharacter == null)
            return;

        if (_pendingNewPatron == null && _pendingAddedVassals.Count == 0 && _pendingRemovedVassals.Count == 0)
        {
            _statusMain.Text = "No allegiance changes to save";
            _statusConnection.Text = "Ready";
            return;
        }

        var loginRecord = _charactersById.Values.FirstOrDefault(r => string.Equals(r.Name, _characterName, StringComparison.OrdinalIgnoreCase));
        if (loginRecord == null)
        {
            MessageBox.Show(this, "Could not find the logged-in character in the downloaded tree. Refresh the tree and try again.", "ACGM Modern", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Cursor = Cursors.WaitCursor;
        try
        {
            var totalBytes = 0;
            _statusConnection.Text = "Connecting to Server";

            if (_pendingNewPatron != null)
            {
                _statusMain.Text = "Changing Patrons";
                var oldPatron = _allegianceCharacter.PatronId > 0 && _charactersById.TryGetValue(_allegianceCharacter.PatronId, out var resolvedOldPatron)
                    ? resolvedOldPatron
                    : null;
                totalBytes += await _protocol.ChangePatronAsync(loginRecord.Id, _allegianceCharacter, oldPatron, _pendingNewPatron);
            }

            foreach (var name in _pendingAddedVassals.ToList())
            {
                _statusMain.Text = "Adding Vassal";
                totalBytes += await _protocol.AddVassalAsync(loginRecord.Id, _allegianceCharacter, name);
            }

            foreach (var vassal in _pendingRemovedVassals.ToList())
            {
                _statusMain.Text = "Removing Vassal";
                totalBytes += await _protocol.RemoveVassalAsync(loginRecord.Id, _allegianceCharacter, vassal);
            }

            _statusBytes.Text = $"{totalBytes} bytes received";
            _statusConnection.Text = "Closing Connection";
            _statusMain.Text = "Allegiance changes saved";
            await RefreshTreeAsync();
        }
        catch (Exception ex)
        {
            _statusMain.Text = "Allegiance save failed";
            _statusConnection.Text = ex.Message;
            AcgmLogger.TryWrite(ex.ToString());
            MessageBox.Show(this, ex.ToString(), "ACGM Modern Allegiance Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private static string? PromptForText(IWin32Window owner, string title, string prompt)
    {
        using var form = new Form
        {
            Text = title,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MinimizeBox = false,
            MaximizeBox = false,
            ClientSize = new Size(380, 118)
        };
        var label = new Label { Text = prompt, Left = 12, Top = 12, Width = 356, Height = 20 };
        var textBox = new TextBox { Left = 12, Top = 38, Width = 356 };
        var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 212, Top = 76, Width = 75 };
        var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Left = 293, Top = 76, Width = 75 };
        form.Controls.AddRange(new Control[] { label, textBox, ok, cancel });
        form.AcceptButton = ok;
        form.CancelButton = cancel;
        return form.ShowDialog(owner) == DialogResult.OK ? textBox.Text : null;
    }

    private static string? PromptForPasswordChange(IWin32Window owner)
    {
        using var form = new Form
        {
            Text = "Change Password",
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MinimizeBox = false,
            MaximizeBox = false,
            ClientSize = new Size(380, 158)
        };

        var passwordLabel = new Label { Text = "New password:", Left = 12, Top = 12, Width = 356, Height = 20 };
        var passwordText = new TextBox { Left = 12, Top = 34, Width = 356, UseSystemPasswordChar = true };
        var confirmLabel = new Label { Text = "Confirm password:", Left = 12, Top = 64, Width = 356, Height = 20 };
        var confirmText = new TextBox { Left = 12, Top = 86, Width = 356, UseSystemPasswordChar = true };
        var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 212, Top = 120, Width = 75 };
        var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Left = 293, Top = 120, Width = 75 };

        form.Controls.AddRange(new Control[] { passwordLabel, passwordText, confirmLabel, confirmText, ok, cancel });
        form.AcceptButton = ok;
        form.CancelButton = cancel;

        while (form.ShowDialog(owner) == DialogResult.OK)
        {
            if (passwordText.Text.Length == 0)
            {
                MessageBox.Show(owner, "Enter a new password.", "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                continue;
            }

            if (!string.Equals(passwordText.Text, confirmText.Text, StringComparison.Ordinal))
            {
                MessageBox.Show(owner, "The passwords do not match.", "Change Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                continue;
            }

            return passwordText.Text;
        }

        return null;
    }

    private async Task FindCharacterAsync()
    {
        if (_charactersById.Count == 0)
        {
            MessageBox.Show(this, "The allegiance tree is not loaded yet. Refresh the tree and try again.", "Find Character", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var search = PromptForText(this, "Find Character", "Enter all or part of the character name:");
        if (string.IsNullOrWhiteSpace(search))
            return;

        search = search.Trim();
        var matches = _charactersById.Values
            .Where(c => c.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Id)
            .ToList();

        AcgmLogger.TryWrite($"Find Character search='{search}', matches={matches.Count}");

        if (matches.Count == 0)
        {
            MessageBox.Show(this, $"No characters matched '{search}'.", "Find Character", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        CharacterRecord? selected = matches.Count == 1
            ? matches[0]
            : ShowFindCharacterSelection(matches);

        if (selected == null)
            return;

        AcgmLogger.TryWrite($"Find Character selected ID={selected.Id}, Name='{selected.Name}'");
        await SelectCharacterRecordAsync(selected);
    }

    private CharacterRecord? ShowFindCharacterSelection(IReadOnlyList<CharacterRecord> matches)
    {
        using var form = new Form
        {
            Text = "Find Character - Select Match",
            FormBorderStyle = FormBorderStyle.Sizable,
            StartPosition = FormStartPosition.CenterParent,
            MinimizeBox = false,
            MaximizeBox = true,
            ClientSize = new Size(460, 340),
            MinimumSize = new Size(360, 260)
        };

        var list = new ListBox
        {
            Dock = DockStyle.Fill,
            DisplayMember = nameof(CharacterRecord.DisplayText),
            IntegralHeight = false
        };
        foreach (var match in matches)
            list.Items.Add(match);
        if (list.Items.Count > 0)
            list.SelectedIndex = 0;

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 44,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 8, 8, 0)
        };
        var ok = new Button { Text = "Select", DialogResult = DialogResult.OK, Width = 84, Height = 28 };
        var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 84, Height = 28 };
        buttons.Controls.Add(ok);
        buttons.Controls.Add(cancel);

        list.DoubleClick += (_, _) =>
        {
            if (list.SelectedItem != null)
                form.DialogResult = DialogResult.OK;
        };

        form.Controls.Add(list);
        form.Controls.Add(buttons);
        form.AcceptButton = ok;
        form.CancelButton = cancel;

        return form.ShowDialog(this) == DialogResult.OK ? list.SelectedItem as CharacterRecord : null;
    }

    private async Task SelectCharacterRecordAsync(CharacterRecord record)
    {
        var node = FindNodeById(_treeView.Nodes, record.Id) ?? FindNodeByName(_treeView.Nodes, record.Name);
        if (node == null)
        {
            MessageBox.Show(this, $"Found {record.Name}, but could not locate the character in the tree view.", "Find Character", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _mainTabs.SelectedIndex = 0;
        ExpandParents(node);
        _treeView.SelectedNode = node;
        node.EnsureVisible();
        await LoadSelectedCharacterAsync();
    }

    private static TreeNode? FindNodeById(TreeNodeCollection nodes, int id)
    {
        foreach (TreeNode node in nodes)
        {
            if (node.Tag is CharacterRecord record && record.Id == id)
                return node;
            var child = FindNodeById(node.Nodes, id);
            if (child != null)
                return child;
        }
        return null;
    }

    private static void ExpandParents(TreeNode node)
    {
        for (var parent = node.Parent; parent != null; parent = parent.Parent)
            parent.Expand();
    }


    private string BuildPathFromTree(CharacterRecord record)
    {
        var names = new List<string> { record.Name };
        var current = record;
        var seen = new HashSet<int> { record.Id };
        while (current.PatronId > 0 && _charactersById.TryGetValue(current.PatronId, out var patron) && seen.Add(patron.Id))
        {
            names.Add(patron.Name);
            current = patron;
        }
        return string.Join(" -> ", names);
    }

    private static string BuildFlagText(CharacterRecord record)
    {
        var flags = new List<string>();
        if (record.IsPk)
            flags.Add("PK");
        if (record.IsMule)
            flags.Add("Mule");
        if (record.IsRescueSquad)
            flags.Add("Rescue");
        return flags.Count == 0 ? string.Empty : string.Join(", ", flags);
    }

    private static string FirstNonEmpty(params string[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }
        return string.Empty;
    }

    private static void PopulateChoiceCombo(ComboBox combo, IReadOnlyList<LegacyChoice> choices, string selectedValue)
    {
        combo.BeginUpdate();
        combo.Items.Clear();
        foreach (var choice in choices)
            combo.Items.Add(choice);
        combo.EndUpdate();
        SetComboByLegacyValue(combo, selectedValue);
    }

    private static void PopulateRankCombo(ComboBox combo, string raceValue, string sexValue, string selectedValue)
    {
        combo.BeginUpdate();
        combo.Items.Clear();
        foreach (var choice in LegacyCharacterParser.GetRankChoices(raceValue, sexValue))
            combo.Items.Add(choice);
        combo.EndUpdate();
        SetComboByLegacyValue(combo, selectedValue);
    }

    private static void SetComboByLegacyValue(ComboBox combo, string selectedValue)
    {
        if (string.IsNullOrWhiteSpace(selectedValue))
        {
            combo.Text = string.Empty;
            return;
        }

        foreach (var item in combo.Items)
        {
            if (item is LegacyChoice choice && string.Equals(choice.Value, selectedValue.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                combo.SelectedItem = choice;
                return;
            }
        }

        combo.Text = selectedValue;
    }


    private void LoadTreeImages()
    {
        if (_treeImages.Images.Count > 0)
            return;

        // Legacy tree icon set. These assets are intentionally small 16x16
        // images to match the VB6 TreeView look while keeping the modern
        // ImageList-based implementation.
        TryAddTreeImage("monarch", "M.bmp");
        TryAddTreeImage("normal", "guy.bmp");
        TryAddTreeImage("pk", "pkico.jpg");
        TryAddTreeImage("mule", "muleico.jpg");
        TryAddTreeImage("rescue", "rescueico.jpg");
    }

    private void TryAddTreeImage(string key, string filename)
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Assets", filename);
            if (File.Exists(path))
            {
                using var image = Image.FromFile(path);
                _treeImages.Images.Add(key, new Bitmap(image, _treeImages.ImageSize));
                return;
            }
        }
        catch (Exception ex)
        {
            AcgmLogger.TryWrite($"Could not load tree icon {filename}: {ex.Message}");
        }
        _treeImages.Images.Add(key, SystemIcons.Application.ToBitmap());
    }

    private static string GetTreeImageKey(CharacterRecord record)
    {
        // Legacy tree icon precedence for the recreated TreeView:
        // Monarch first, then Mule, then Rescue Squad, then PK, then normal
        // character. The legacy tree marks the monarch/root with PatronId -1.
        if (record.PatronId == -1)
            return "monarch";
        if (record.IsMule)
            return "mule";
        if (record.IsRescueSquad)
            return "rescue";
        if (record.IsPk)
            return "pk";
        return "normal";
    }

    private void SelectDetailTab(int index)
    {
        if (index < 0 || index >= _detailTabs.TabPages.Count)
            return;
        _mainTabs.SelectedIndex = 0;
        _detailTabs.SelectedIndex = index;
    }

    private void SaveWindowLayoutNow()
    {
        SaveWindowState();
        SaveSplitterDistance();
        SaveSelectedTab();
        _statusMain.Text = "Window layout saved";
        _statusConnection.Text = "Ready";
    }

    private void ResetWindowLayout()
    {
        var response = MessageBox.Show(this, "Reset saved window size, position, splitter, and selected detail tab?", "Reset Window Layout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (response != DialogResult.Yes)
            return;

        _settings.MainWindowLeft = -1;
        _settings.MainWindowTop = -1;
        _settings.MainWindowWidth = 980;
        _settings.MainWindowHeight = 650;
        _settings.MainSplitterDistance = 0;
        _settings.SelectedDetailTabIndex = 0;
        _settings.Save();

        _detailTabs.SelectedIndex = 0;
        Size = new Size(Math.Max(820, _settings.MainWindowWidth), Math.Max(580, _settings.MainWindowHeight));
        StartPosition = FormStartPosition.CenterScreen;
        BeginInvoke(new Action(ApplyInitialSplitterDistance));
        _statusMain.Text = "Window layout reset";
        _statusConnection.Text = "Restart the client to fully recenter the window";
    }

    private void OpenLogsFolder()
    {
        OpenFolder(AppContext.BaseDirectory);
    }

    private void OpenDiagnosticsFolder()
    {
        OpenFolder(AcgmDiagnostics.DiagnosticsDirectory);
    }

    private void OpenFolder(string path)
    {
        try
        {
            Directory.CreateDirectory(path);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            AcgmLogger.TryWrite(ex.ToString());
            MessageBox.Show(this, ex.Message, "Open Folder Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    private CharacterRecord? GetLoggedInCharacterRecord()
    {
        return _charactersById.Values.FirstOrDefault(r => string.Equals(r.Name, _characterName, StringComparison.OrdinalIgnoreCase));
    }

    private async Task AdminBackupDatabaseAsync()
    {
        var loginRecord = GetLoggedInCharacterRecord();
        if (loginRecord == null)
        {
            MessageBox.Show(this, "Could not find the logged-in character in the downloaded tree. Refresh the tree and try again.", "Backup Database", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Title = "Backup Character Database",
            Filter = "Tree Data Files (*.dat)|*.dat|All Files (*.*)|*.*",
            FileName = "tree-backup.dat",
            AddExtension = true,
            DefaultExt = "dat",
            OverwritePrompt = true
        };

        if (dialog.ShowDialog(this) != DialogResult.OK || string.IsNullOrWhiteSpace(dialog.FileName))
            return;

        _statusMain.Text = "Backing up character database";
        _statusConnection.Text = "Connecting to Server";
        Cursor = Cursors.WaitCursor;
        try
        {
            var result = await _protocol.BackupDatabaseAsync(loginRecord.Id);
            File.WriteAllText(dialog.FileName, result.Payload);
            _statusBytes.Text = $"{result.Bytes} bytes received";
            _statusConnection.Text = "Closing Connection";
            _statusMain.Text = "Database backup saved";
            MessageBox.Show(this, "The character database has been backed up.", "Backup Database", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _statusMain.Text = "Backup failed";
            _statusConnection.Text = ex.Message;
            AcgmLogger.TryWrite(ex.ToString());
            MessageBox.Show(this, ex.ToString(), "ACGM Modern Backup Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private async Task AdminResetCharacterPasswordAsync()
    {
        var loginRecord = GetLoggedInCharacterRecord();
        if (loginRecord == null)
        {
            MessageBox.Show(this, "Could not find the logged-in character in the downloaded tree. Refresh the tree and try again.", "Reset Character Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var target = SelectedCharacter;
        if (target == null)
        {
            MessageBox.Show(this, "You must select the character whose password you want to reset from the tree.", "Reset Character Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var response = MessageBox.Show(this,
            $"Are you sure you want to reset {target.Name}'s password to the default password?",
            "Reset Password Confirmation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (response != DialogResult.Yes)
            return;

        _statusMain.Text = "Resetting character password";
        _statusConnection.Text = "Connecting to Server";
        Cursor = Cursors.WaitCursor;
        try
        {
            var bytes = await _protocol.ResetCharacterPasswordAsync(loginRecord.Id, target.Id);
            _statusBytes.Text = $"{bytes} bytes received";
            _statusConnection.Text = "Closing Connection";
            _statusMain.Text = "Password reset";
            MessageBox.Show(this, "The character's password has been reset to the default password.", "Reset Character Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _statusMain.Text = "Password reset failed";
            _statusConnection.Text = ex.Message;
            AcgmLogger.TryWrite(ex.ToString());
            MessageBox.Show(this, ex.ToString(), "ACGM Modern Password Reset Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private async Task AdminChangeSecurityLevelAsync()
    {
        var loginRecord = GetLoggedInCharacterRecord();
        if (loginRecord == null)
        {
            MessageBox.Show(this, "Could not find the logged-in character in the downloaded tree. Refresh the tree and try again.", "Change Security Level", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var target = SelectedCharacter;
        if (target == null)
        {
            MessageBox.Show(this, "You must select the character whose security level you want to change.", "Change Security Level", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var currentLevel = GetKnownSecurityLevelForCharacter(target);
        var selection = ShowSecurityLevelDialog(target.Name, currentLevel);
        if (selection == null)
            return;

        var (levelName, levelValue) = selection.Value;
        var response = MessageBox.Show(this,
            $"Are you sure you want to change {target.Name}'s security level to {levelName}?",
            "Change Security Level Confirmation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (response != DialogResult.Yes)
            return;

        _statusMain.Text = "Changing security level";
        _statusConnection.Text = "Connecting to Server";
        Cursor = Cursors.WaitCursor;
        try
        {
            var bytes = await _protocol.ChangeSecurityLevelAsync(loginRecord.Id, target.Id, levelValue);
            _statusBytes.Text = $"{bytes} bytes received";
            _statusConnection.Text = "Closing Connection";
            _statusMain.Text = "Security level changed";
            MessageBox.Show(this, "The character's security level has been changed!", "Change Security Level", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _statusMain.Text = "Security level change failed";
            _statusConnection.Text = ex.Message;
            AcgmLogger.TryWrite(ex.ToString());
            MessageBox.Show(this, ex.ToString(), "ACGM Modern Security Level Change Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private int GetKnownSecurityLevelForCharacter(CharacterRecord target)
    {
        // The legacy client only exposes the current login security level from
        // the login response. Use that value when changing the logged-in
        // character so administrators do not see themselves defaulted to
        // Normal User. For other characters, keep the safe legacy default.
        var loginRecord = GetLoggedInCharacterRecord();
        if (loginRecord != null && loginRecord.Id == target.Id)
            return _currentSecurityLevel;

        if (string.Equals(target.Name, _characterName, StringComparison.OrdinalIgnoreCase))
            return _currentSecurityLevel;

        return 1;
    }

    private (string Name, int Value)? ShowSecurityLevelDialog(string characterName, int currentLevel)
    {
        using var form = new Form
        {
            Text = "Change Security Level",
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MinimizeBox = false,
            MaximizeBox = false,
            ShowIcon = false,
            ShowInTaskbar = false,
            ClientSize = new Size(360, 138)
        };

        var label = new Label
        {
            Text = $"Security level for {characterName}:",
            Left = 12,
            Top = 14,
            Width = 330,
            Height = 22,
            Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold)
        };
        var combo = new ComboBox
        {
            Left = 12,
            Top = 44,
            Width = 330,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        combo.Items.Add("Normal User");
        combo.Items.Add("Administrator");
        combo.SelectedIndex = currentLevel == 3 ? 1 : 0;

        var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 186, Top = 92, Width = 75 };
        var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Left = 267, Top = 92, Width = 75 };
        form.Controls.AddRange(new Control[] { label, combo, ok, cancel });
        form.AcceptButton = ok;
        form.CancelButton = cancel;

        if (form.ShowDialog(this) != DialogResult.OK)
            return null;

        var name = Convert.ToString(combo.SelectedItem) ?? "Normal User";
        return name == "Administrator" ? (name, 3) : (name, 1);
    }


    private async Task AdminServerSetupAsync()
    {
        var options = ShowServerSetupDialog();
        if (options == null)
            return;

        var response = MessageBox.Show(this,
            "Save the new ACGM server setup?\n\nIf you changed Rescue Squad settings, restart ACGM to see all label/menu changes.",
            "Server Setup Confirmation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (response != DialogResult.Yes)
            return;

        _statusMain.Text = "Saving server setup";
        _statusConnection.Text = "Connecting to Server";
        Cursor = Cursors.WaitCursor;
        try
        {
            var bytes = await _protocol.ChangeServerSetupAsync(options);
            _statusBytes.Text = $"{bytes} bytes received";
            _statusConnection.Text = "Closing Connection";
            _statusMain.Text = "Server setup saved";
            _guildName = options.GuildName;
            _useRescueSquad = options.UseRescueSquad;
            _rescueSquadName = options.RescueSquadName;
            if (!string.IsNullOrWhiteSpace(_guildName))
                UpdateWindowTitle();
            MessageBox.Show(this, "The server has saved the new setup!", "Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _statusMain.Text = "Server setup failed";
            _statusConnection.Text = ex.Message;
            AcgmLogger.TryWrite(ex.ToString());
            MessageBox.Show(this, ex.ToString(), "ACGM Modern Server Setup Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private ServerSetupOptions? ShowServerSetupDialog()
    {
        using var form = new Form
        {
            Text = "Setup ACGM Server",
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MinimizeBox = false,
            MaximizeBox = false,
            ShowIcon = false,
            ShowInTaskbar = false,
            ClientSize = new Size(520, 394)
        };

        var guildLabel = new Label
        {
            Text = "Guild Name:",
            Left = 16,
            Top = 18,
            Width = 170,
            Height = 22,
            Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold)
        };
        var guildText = new TextBox
        {
            Left = 205,
            Top = 15,
            Width = 285,
            Text = _guildName
        };

        var rescueBox = new GroupBox
        {
            Text = "Rescue Squad",
            Left = 12,
            Top = 52,
            Width = 492,
            Height = 116,
            Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold)
        };
        var useRescue = new CheckBox
        {
            Text = "Use Rescue Squad",
            Left = 14,
            Top = 26,
            Width = 180,
            Checked = _useRescueSquad,
            Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold)
        };
        var rescueNameLabel = new Label
        {
            Text = "Rescue Squad Name:",
            Left = 14,
            Top = 58,
            Width = 170,
            Height = 22,
            Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold)
        };
        var rescueNameText = new TextBox
        {
            Left = 205,
            Top = 55,
            Width = 265,
            Text = _rescueSquadName,
            Enabled = _useRescueSquad
        };
        var rescueNote = new Label
        {
            Text = "If you enable or disable the Rescue Squad feature, exit ACGM and re-enter to see all changes.",
            Left = 14,
            Top = 84,
            Width = 460,
            Height = 22,
            Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Regular)
        };
        useRescue.CheckedChanged += (_, _) => rescueNameText.Enabled = useRescue.Checked;
        rescueBox.Controls.AddRange(new Control[] { useRescue, rescueNameLabel, rescueNameText, rescueNote });

        var passwordBox = new GroupBox
        {
            Text = "Default Password",
            Left = 12,
            Top = 178,
            Width = 492,
            Height = 152,
            Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold)
        };
        var passwordNote = new Label
        {
            Text = "Leave the default password blank to keep the current server default password unchanged.",
            Left = 14,
            Top = 24,
            Width = 455,
            Height = 34,
            Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Regular)
        };
        var defaultLabel = new Label
        {
            Text = "Default Password:",
            Left = 14,
            Top = 68,
            Width = 180,
            Height = 22,
            Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold)
        };
        var defaultText = new TextBox
        {
            Left = 205,
            Top = 65,
            Width = 265,
            UseSystemPasswordChar = true,
            MaxLength = 10
        };
        var confirmLabel = new Label
        {
            Text = "Confirm Default Password:",
            Left = 14,
            Top = 98,
            Width = 190,
            Height = 22,
            Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold)
        };
        var confirmText = new TextBox
        {
            Left = 205,
            Top = 95,
            Width = 265,
            UseSystemPasswordChar = true,
            MaxLength = 10
        };
        var resetExisting = new CheckBox
        {
            Text = "Reset all users with current default password to new default password",
            Left = 14,
            Top = 124,
            Width = 455,
            Height = 22,
            Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Regular)
        };
        passwordBox.Controls.AddRange(new Control[] { passwordNote, defaultLabel, defaultText, confirmLabel, confirmText, resetExisting });

        var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 130, Top = 346, Width = 110, Height = 30 };
        var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Left = 280, Top = 346, Width = 110, Height = 30 };
        form.Controls.AddRange(new Control[] { guildLabel, guildText, rescueBox, passwordBox, ok, cancel });
        form.AcceptButton = ok;
        form.CancelButton = cancel;

        while (true)
        {
            if (form.ShowDialog(this) != DialogResult.OK)
                return null;

            var guild = guildText.Text.Trim();
            var defaultPassword = defaultText.Text;
            var confirmPassword = confirmText.Text;
            var rescueName = rescueNameText.Text.Trim();

            if (guild.Length == 0)
            {
                MessageBox.Show(form, "You must enter your guild's name!", "Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guildText.Focus();
                continue;
            }
            if (confirmPassword.Length == 0 && defaultPassword.Length > 0)
            {
                MessageBox.Show(form, "You must confirm the password you entered for the default password!", "Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                confirmText.Focus();
                continue;
            }
            if (!string.Equals(confirmPassword, defaultPassword, StringComparison.Ordinal))
            {
                MessageBox.Show(form, "The confirmation password does not match the password you entered!", "Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                confirmText.Focus();
                continue;
            }
            if (defaultPassword.Length > 0 && defaultPassword.Length < 6)
            {
                MessageBox.Show(form, "Your password is too short!  It must be at least 6 characters long.", "Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                defaultText.Focus();
                continue;
            }
            if (defaultPassword.Length > 10)
            {
                MessageBox.Show(form, "Your password is too long!  It can be at most 10 characters long.", "Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                defaultText.Focus();
                continue;
            }
            if (useRescue.Checked && rescueName.Length == 0)
            {
                MessageBox.Show(form, "You must specify the name of your rescue squad!", "Server Setup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rescueNameText.Focus();
                continue;
            }

            return new ServerSetupOptions
            {
                GuildName = guild,
                DefaultPassword = defaultPassword,
                ResetExistingDefaultPasswordUsers = resetExisting.Checked,
                UseRescueSquad = useRescue.Checked,
                RescueSquadName = rescueName
            };
        }
    }

    private void ShowNotYetRestored(string message)
    {
        MessageBox.Show(this, message, "ACGM Modern", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ShowLegacyAdministratorPending(string functionName)
    {
        MessageBox.Show(this,
            functionName + " has not yet been restored. The legacy Administrator menu entry is now present, but the server-side behavior and CGI protocol still need to be confirmed before this action can safely modify server data.",
            "Administrator",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }


    private void ShowAdministratorFunctionsDialog()
    {
        using var dialog = new Form
        {
            Text = "Administrator Functions",
            StartPosition = FormStartPosition.CenterParent,
            MinimizeBox = false,
            MaximizeBox = false,
            ShowIcon = false,
            ShowInTaskbar = false,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Width = 560,
            Height = 420
        };

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            ColumnCount = 1,
            RowCount = 4
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        dialog.Controls.Add(root);

        root.Controls.Add(new Label
        {
            Text = "Legacy Administrator Functions",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold)
        }, 0, 0);

        var list = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            HideSelection = false,
            MultiSelect = false
        };
        list.Columns.Add("Function", 220);
        list.Columns.Add("Status", 110);
        list.Columns.Add("Notes", 190);
        AddAdministratorFunction(list, "Refresh Tree", "Restored", "Uses existing tree download");
        AddAdministratorFunction(list, "Find Character", "Restored", "Uses loaded tree records");
        AddAdministratorFunction(list, "Save Current Character", "Restored", "Uses verified 0.11.4 save protocol");
        AddAdministratorFunction(list, "Edit Character Flags", "Restored", "Available on Basic Info tab");
        AddAdministratorFunction(list, "Awards", "Restored", "Available on Awards tab");
        AddAdministratorFunction(list, "Rescue Squad List", "Restored", "Available on main tab");
        AddAdministratorFunction(list, "Portal List", "Restored", "Available on main tab");
        AddAdministratorFunction(list, "Trade Skills List", "Restored", "Available on main tab");
        AddAdministratorFunction(list, "Server-side admin tools", "Pending", "Legacy behavior not confirmed yet");
        root.Controls.Add(list, 0, 1);

        var notes = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Text = "Administrator Functions are being restored safely. Restored actions use existing verified client workflows. Pending server-side administration remains disabled until the VB6 behavior and CGI protocol are confirmed."
        };
        root.Controls.Add(notes, 0, 2);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0)
        };
        var closeButton = new Button { Text = "Close", Width = 100, Height = 30, DialogResult = DialogResult.OK };
        var runButton = new Button { Text = "Open Selected", Width = 120, Height = 30 };
        runButton.Click += async (_, _) => await RunSelectedAdministratorFunctionAsync(list, dialog);
        buttons.Controls.Add(closeButton);
        buttons.Controls.Add(runButton);
        root.Controls.Add(buttons, 0, 3);

        dialog.AcceptButton = runButton;
        dialog.CancelButton = closeButton;
        dialog.ShowDialog(this);
    }

    private static void AddAdministratorFunction(ListView list, string name, string status, string notes)
    {
        var item = new ListViewItem(name) { Tag = name };
        item.SubItems.Add(status);
        item.SubItems.Add(notes);
        list.Items.Add(item);
    }

    private async Task RunSelectedAdministratorFunctionAsync(ListView list, Form dialog)
    {
        if (list.SelectedItems.Count == 0)
        {
            MessageBox.Show(dialog, "Select an administrator function first.", "Administrator Functions", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var functionName = list.SelectedItems[0].Tag as string ?? string.Empty;
        switch (functionName)
        {
            case "Refresh Tree":
                dialog.Close();
                await RefreshTreeAsync();
                break;
            case "Find Character":
                dialog.Close();
                await FindCharacterAsync();
                break;
            case "Save Current Character":
                dialog.Close();
                await SaveCurrentCharacterAsync();
                break;
            case "Edit Character Flags":
                dialog.Close();
                SelectDetailTab(0);
                break;
            case "Awards":
                dialog.Close();
                SelectDetailTab(4);
                break;
            case "Rescue Squad List":
                dialog.Close();
                _mainTabs.SelectedIndex = 2;
                await RefreshRescueSquadAsync();
                break;
            case "Portal List":
                dialog.Close();
                _mainTabs.SelectedIndex = 3;
                await RefreshPortalListAsync();
                break;
            case "Trade Skills List":
                dialog.Close();
                _mainTabs.SelectedIndex = 4;
                await RefreshTradeSkillsListAsync();
                break;
            default:
                MessageBox.Show(dialog, "This server-side administrator function has not yet been restored because the legacy VB6 behavior and CGI protocol still need to be confirmed.", "Administrator Functions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                break;
        }
    }

    private void ShowAboutDialog()
    {
        MessageBox.Show(this,
            "ACGM Modern Client 0.11.10a\n\nSecurity Level Dialog Default Fix\n\nRecreation first. Modernization underneath.",
            "About ACGM Modern Client",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void ResetCurrentCharacter()
    {
        if (_currentDetails == null || _currentRecord == null)
            return;

        DisplayCharacterDetails(_currentDetails, _currentRecord);
        _statusMain.Text = "Changes reset";
        _statusConnection.Text = "Ready";
    }

    private async Task SaveCurrentCharacterAsync()
    {
        if (_currentDetails == null || _currentRecord == null)
        {
            MessageBox.Show(this, "Select a character before saving changes.", "ACGM Modern", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var updated = BuildDetailsFromControls(_currentDetails, _currentRecord);
        if (!HasBasicInfoChanges(_currentDetails, updated))
        {
            _statusMain.Text = "No changes to save";
            _statusConnection.Text = "Ready";
            return;
        }

        var loginRecord = _charactersById.Values.FirstOrDefault(r => string.Equals(r.Name, _characterName, StringComparison.OrdinalIgnoreCase));
        if (loginRecord == null)
        {
            MessageBox.Show(this, "Could not find the logged-in character in the downloaded tree. Refresh the tree and try again.", "ACGM Modern", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _statusMain.Text = "Saving Character Info";
        _statusConnection.Text = "Connecting to Server";
        Cursor = Cursors.WaitCursor;
        try
        {
            var bytes = await _protocol.SaveCharacterInfoAsync(loginRecord.Id, _currentRecord.Id, _currentDetails, updated);
            _statusBytes.Text = $"{bytes} bytes received";
            _statusConnection.Text = "Closing Connection";
            _statusMain.Text = "Character info saved";
            await LoadSelectedCharacterAsync();
        }
        catch (Exception ex)
        {
            _statusMain.Text = "Save failed";
            _statusConnection.Text = ex.Message;
            AcgmLogger.TryWrite(ex.ToString());

            if (ex.Message.Contains("972", StringComparison.Ordinal))
            {
                MessageBox.Show(this,
                    "There was an error updating the character info.  You attempted to edit a field that only admins are allowed to edit.",
                    "ACGM Error #972",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(this, ex.ToString(), "ACGM Modern Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private CharacterDetails BuildDetailsFromControls(CharacterDetails original, CharacterRecord fallback)
    {
        return new CharacterDetails
        {
            Id = original.Id == 0 ? fallback.Id : original.Id,
            RawFieldCount = original.RawFieldCount,
            Name = _nameText.Text.Trim(),
            Level = _levelText.Text.Trim(),
            ClassName = _classText.Text.Trim(),
            Rank = GetComboLegacyValue(_rankCombo),
            ComputedRank = GetComboLegacyValue(_computedRankCombo),
            Race = GetComboLegacyValue(_raceCombo),
            Sex = GetComboLegacyValue(_sexCombo),
            IsMule = _muleYes.Checked,
            MuleFor = _muleForText.Text.Trim(),
            MainCharacterName = _mainCharacterText.Text.Trim(),
            LifestonedAt = _lifestoneText.Text.Trim(),
            TiedTo = _tiedToText.Text.Trim(),
            CanSummon = _canSummonCheck.Checked,
            IsPk = _pkCheck.Checked,
            IsRescueMember = _rescueCheck.Checked,
            HideInfoOnWeb = _hideInfoCheck.Checked,
            Bio = _bioText.Text,
            Skills = BuildSkillsFromControls(),
            RealName = _realNameText.Text.Trim(),
            CityState = _cityStateText.Text.Trim(),
            MiscRealLife = _miscRealLifeText.Text,
            Email = _emailText.Text.Trim(),
            Icq = _icqText.Text.Trim(),
            Patron = original.Patron,
            PathToMonarch = original.PathToMonarch,
            LastModified = original.LastModified,
            LastModifiedBy = original.LastModifiedBy,
            LastModifiedByCharacterId = original.LastModifiedByCharacterId,
            Awards = _awardsText.Text
        };
    }

    private static string GetComboLegacyValue(ComboBox combo)
    {
        if (combo.SelectedItem is LegacyChoice choice)
            return choice.Value;
        return combo.Text.Trim();
    }

    private static bool HasBasicInfoChanges(CharacterDetails oldValue, CharacterDetails newValue)
    {
        return !Same(oldValue.Name, newValue.Name)
            || !Same(oldValue.Level, newValue.Level)
            || !Same(oldValue.ClassName, newValue.ClassName)
            || !Same(oldValue.Rank, newValue.Rank)
            || !Same(oldValue.Race, newValue.Race)
            || !Same(oldValue.Sex, newValue.Sex)
            || oldValue.IsMule != newValue.IsMule
            || !Same(oldValue.MuleFor, newValue.MuleFor)
            || !Same(oldValue.MainCharacterName, newValue.MainCharacterName)
            || !Same(oldValue.LifestonedAt, newValue.LifestonedAt)
            || !Same(oldValue.TiedTo, newValue.TiedTo)
            || oldValue.CanSummon != newValue.CanSummon
            || oldValue.IsPk != newValue.IsPk
            || oldValue.IsRescueMember != newValue.IsRescueMember
            || oldValue.HideInfoOnWeb != newValue.HideInfoOnWeb
            || !Same(oldValue.Bio, newValue.Bio)
            || !Same(oldValue.Skills, newValue.Skills)
            || !Same(oldValue.RealName, newValue.RealName)
            || !Same(oldValue.CityState, newValue.CityState)
            || !Same(oldValue.MiscRealLife, newValue.MiscRealLife)
            || !Same(oldValue.Email, newValue.Email)
            || !Same(oldValue.Icq, newValue.Icq)
            || !Same(oldValue.Awards, newValue.Awards);
    }

    private static bool Same(string? left, string? right)
    {
        return string.Equals((left ?? string.Empty).Trim(), (right ?? string.Empty).Trim(), StringComparison.Ordinal);
    }

    private void SaveSelectedTab()
    {
        if (!_windowStateReady || _detailTabs.SelectedIndex < 0)
            return;
        _settings.SelectedDetailTabIndex = _detailTabs.SelectedIndex;
        _settings.Save();
    }

    private void RestoreWindowState()
    {
        if (_settings.MainWindowWidth >= MinimumSize.Width && _settings.MainWindowHeight >= MinimumSize.Height)
        {
            Size = new Size(_settings.MainWindowWidth, _settings.MainWindowHeight);
        }

        if (_settings.MainWindowLeft >= 0 && _settings.MainWindowTop >= 0)
        {
            var requested = new Rectangle(_settings.MainWindowLeft, _settings.MainWindowTop, Width, Height);
            if (Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(requested)))
            {
                StartPosition = FormStartPosition.Manual;
                Location = requested.Location;
            }
        }
    }

    private void SaveWindowState()
    {
        if (!_windowStateReady || WindowState != FormWindowState.Normal)
            return;

        _settings.MainWindowLeft = Left;
        _settings.MainWindowTop = Top;
        _settings.MainWindowWidth = Width;
        _settings.MainWindowHeight = Height;
        _settings.Save();
    }
}
