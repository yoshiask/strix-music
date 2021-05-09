﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Utils;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.Notifications;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// Configures the MusicBrainz core.
    /// </summary>
    public class MusicBrainzCoreConfig : ICoreConfig
    {
        private IFileSystemService? _fileSystemService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzCoreConfig"/> class.
        /// </summary>
        public MusicBrainzCoreConfig(ICore sourceCore)
        {
            SourceCore = sourceCore;

            var textBlock = new AbstractTextBox("testBox", "The initial value")
            {
                Title = "Password entry",
                Subtitle = @"Enter ""something useful"".",
            };

            textBlock.ValueChanged += TextBlock_ValueChanged;

            var button = new AbstractButton(Guid.NewGuid().ToString(), "Pick folder")
            {
                Title = "Add a folder",
                IconCode = "\uE2B1",
            };

            button.Clicked += Button_Clicked;

            var allDoneButton = new AbstractButton(Guid.NewGuid().ToString(), "Done")
            {
                IconCode = "\uE73E",
            };

            /*var richTextblock = new AbstractRichTextBlock(Guid.NewGuid().ToString(), "The initial value")
            {
                Title = "RichTextBlock Example",
                IconCode = "\uE2B1",
            };*/

            allDoneButton.Clicked += AllDoneButton_Clicked;

            var dataListItems = new List<AbstractUIMetadata>
            {
                new AbstractUIMetadata(Guid.NewGuid().ToString())
                {
                    Title = "Item 1",
                    Subtitle = "Subtitle: The test",
                    ImagePath = "https://image.redbull.com/rbcom/052/2017-06-19/3965fbe6-3488-40f8-88bc-b82eb8d1a230/0010/1/800/800/1/pogchamp-twitch.png",
                },
                new AbstractUIMetadata(Guid.NewGuid().ToString())
                {
                    Title = "Item 2",
                    Subtitle = "Subtitle: The sequel",
                    IconCode = "\uE2B1",
                },
                new AbstractUIMetadata(Guid.NewGuid().ToString())
                {
                    Title = "Item 3",
                    IconCode = "\uE7F6",
                },
                new AbstractUIMetadata(Guid.NewGuid().ToString())
                {
                    Title = "Item 4",
                    IconCode = "\uE753",
                },
            };

            var dataListGrid = new AbstractDataList(id: "dataListGrid", dataListItems.ToList())
            {
                Title = "DataList grid test",
                Subtitle = "Add or remove something",
                PreferredDisplayMode = AbstractDataListPreferredDisplayMode.Grid,
                IsUserEditingEnabled = false,
            };

            var dataList = new AbstractDataList(id: "dataList", dataListItems.ToList())
            {
                Title = "DataList test",
                Subtitle = "Add or remove something",
                PreferredDisplayMode = AbstractDataListPreferredDisplayMode.List,
                IsUserEditingEnabled = false,
            };

            dataListGrid.AddRequested += DataListGrid_AddRequested;
            dataList.AddRequested += DataListGrid_AddRequested;
            dataList.ItemTapped += DataList_ItemTapped;

            var multiChoiceItems = dataListItems.ToList();

            var comboBox = new AbstractMultiChoiceUIElement(id: "comboBoxTest", multiChoiceItems[0], multiChoiceItems)
            {
                Title = "ComboBox test",
            };

            var radioButtons = new AbstractMultiChoiceUIElement(id: "radioButtonsTest", multiChoiceItems[0], multiChoiceItems)
            {
                Title = "RadioButtons test",
                PreferredDisplayMode = AbstractMultiChoicePreferredDisplayMode.RadioButtons,
            };

            comboBox.ItemSelected += ComboBox_ItemSelected;
            radioButtons.ItemSelected += ComboBox_ItemSelected;

            var boolUi = new AbstractBooleanUIElement("booleanTest", "On")
            {
                State = true,
                Title = "AbstractBoolean test",
            };

            var buttons = new AbstractUIElementGroup("OkOrCancel", PreferredOrientation.Horizontal)
            {
                Items = new List<AbstractUIElement>()
                {
                    new AbstractButton("OkButton", "Ok", "\uE001"),
                    new AbstractButton("CanelButton", "Cancel", "\uE10A"),
                },
            };

            boolUi.StateChanged += BoolUi_StateChanged;
            boolUi.StateChanged += (sender, b) =>
            {
                dataListGrid.IsUserEditingEnabled = boolUi.State;
            };

            AbstractUIElements = new List<AbstractUIElementGroup>
            {
                new AbstractUIElementGroup("about", PreferredOrientation.Horizontal)
                {
                    Title = "MusicBrainz Core",
                    Subtitle = "AbstractUI Demo",
                    Items =  new List<AbstractUIElement>()
                    {
                        textBlock,
                        boolUi,
                        comboBox,
                        radioButtons,
                        dataList,
                        dataListGrid,
                        button,
                        buttons,
                        allDoneButton,
                    },
                },
            };
        }

        private void DataList_ItemTapped(object sender, AbstractUIMetadata e)
        {
            var notifServ = Ioc.Default.GetRequiredService<INotificationService>();
            notifServ.RaiseNotification("Wow", $"You tapped {e.Title}");
        }

        private void BoolUi_StateChanged(object sender, bool e)
        {
            if (sender is AbstractBooleanUIElement boolUi)
            {
                boolUi.ChangeLabel(e ? "On" : "Off");
            }
        }

        private void ComboBox_ItemSelected(object sender, AbstractUIMetadata e)
        {
            if (e.Title == "Item 3")
            {
                ((MusicBrainzCore)SourceCore).ChangeCoreState(Sdk.Data.CoreState.Configured);
            }
        }

        private async void DataListGrid_AddRequested(object sender, EventArgs e)
        {
            if (!(sender is AbstractDataList dataList))
                return;

            Guard.IsNotNull(_fileSystemService, nameof(_fileSystemService));

            var folder = await _fileSystemService.PickFolder();

            if (folder is null)
                return;

            var newItem = new AbstractUIMetadata(Guid.NewGuid().ToString())
            {
                IconCode = "\uED25",
                Title = folder.Name,
                Subtitle = folder.Path,
                TooltipText = folder.Path,
            };

            dataList.AddItem(newItem);
        }

        private void AllDoneButton_Clicked(object sender, EventArgs e)
        {
            if (SourceCore is MusicBrainzCore core)
            {
                core.ChangeCoreState(Sdk.Data.CoreState.Configured);
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            Guard.IsNotNull(_fileSystemService, nameof(_fileSystemService));

            var folder = await _fileSystemService.PickFolder();

            if (folder != null)
            {
                Debug.WriteLine(folder.Name);
                Debug.WriteLine(folder.Path);
            }
            else
            {
                Debug.WriteLine("Nothing picked");
            }
        }

        private void TextBlock_ValueChanged(object sender, string e)
        {
            if (e == "something useful")
            {
                ((MusicBrainzCore)SourceCore).ChangeCoreState(Sdk.Data.CoreState.Configured);
            }
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public IServiceProvider? Services { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<AbstractUIElementGroup> AbstractUIElements { get; }

        /// <inheritdoc />
        public MediaPlayerType PlaybackType => MediaPlayerType.None;

        /// <inheritdoc />
        public event EventHandler? AbstractUIElementsChanged;

        /// <summary>
        /// Configures services for this instance of the core.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ConfigureServices(IServiceCollection services)
        {
            Services = null;

            var cacheService = new MusicBrainzCacheService();
            await cacheService.InitAsync();

            var musicBrainzClient = new MusicBrainzClient
            {
                Cache = new FileRequestCache(cacheService.RootFolder.Path),
            };

            var musicBrainzArtistHelper = new MusicBrainzArtistHelpersService(musicBrainzClient);

            services.Add(new ServiceDescriptor(typeof(MusicBrainzClient), musicBrainzClient));
            services.Add(new ServiceDescriptor(typeof(MusicBrainzArtistHelpersService), musicBrainzArtistHelper));

            Services = services.BuildServiceProvider();
        }

        /// <summary>
        /// Configures the minimum required services for core configuration in a safe manner and is guaranteed not to throw.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetupConfigurationServices(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();

            _fileSystemService = provider.GetRequiredService<IFileSystemService>();
            return _fileSystemService.InitAsync();
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
