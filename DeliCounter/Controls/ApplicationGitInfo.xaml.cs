﻿using System;
using System.Windows;
using System.Windows.Controls;
using DeliCounter.Backend;
using Semver;

namespace DeliCounter.Controls
{
    public partial class ApplicationGitInfo : UserControl
    {
        public ApplicationGitInfo()
        {
            InitializeComponent();
            BuildInfo.Text = Text;
            ModRepository.Instance.RepositoryUpdated += ModRepositoryUpdated;
        }

        private void ModRepositoryUpdated()
        {
            var applicationData = ModRepository.Instance.ApplicationData;
            var cVer = SemVersion.Parse($"{ThisAssembly.Git.SemVer.Major}.{ThisAssembly.Git.SemVer.Minor}.{ThisAssembly.Git.SemVer.Patch}");
            if (applicationData.LatestApplicationVersion > cVer)
            {
                App.RunInMainThread(() =>
                {
                    UpdateText.Visibility = Visibility.Visible;
                    UpdateText.Text = $"Update to {applicationData.LatestApplicationVersion} available!\n{applicationData.UpdateText}";
                });
            }
        }

        public static string Text
        {
            get
            {
                var commitDate = DateTime.Parse(ThisAssembly.Git.CommitDate);
                return $"Tag: {ThisAssembly.Git.Tag}\n" +
                       $"Branch: {ThisAssembly.Git.Branch}{(ThisAssembly.Git.IsDirty ? "-dirty" : "")} ({ThisAssembly.Git.Commit})\n" +
                       $"Commit Date: {commitDate}";
            }
        }
    }
}