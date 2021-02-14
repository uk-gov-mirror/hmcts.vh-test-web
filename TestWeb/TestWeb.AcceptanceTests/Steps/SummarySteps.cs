﻿using AcceptanceTests.Common.Driver.Drivers;
using AcceptanceTests.Common.Driver.Helpers;
using AcceptanceTests.Common.Test.Steps;
using FluentAssertions;
using TechTalk.SpecFlow;
using TestWeb.AcceptanceTests.Data;
using TestWeb.AcceptanceTests.Helpers;
using TestWeb.AcceptanceTests.Pages;

namespace TestWeb.AcceptanceTests.Steps
{
    [Binding]
    public class SummarySteps : ISteps
    {
        private readonly UserBrowser _browser;
        private readonly TestContext _c;
        private readonly BrowserSteps _browserSteps;
        private const int JudgesCount = 1;

        public SummarySteps(UserBrowser browser, TestContext testContext, BrowserSteps browserSteps)
        {
            _browser = browser;
            _c = testContext;
            _browserSteps = browserSteps;
        }

        [Then(@"the summary page displays the new hearing details")]
        public void ThenTheSummaryPageDisplaysTheNewHearingDetails()
        {
            for (var i = 0; i < _c.Test.CaseNames.Count; i++)
            {
                _browser.Driver.WaitUntilVisible(SummaryPage.CaseName(i)).Text.Trim().Should().Be(_c.Test.CaseNames[i]);
                _browser.Driver.WaitUntilVisible(SummaryPage.CaseNumber(i)).Text.Trim().Should().NotBeNull();
                _browser.Driver.WaitUntilVisible(SummaryPage.ScheduledDate(i)).Text.Trim().Should().NotBeNull();
                _browser.Driver.WaitUntilVisible(SummaryPage.HearingId(i)).Text.Trim().Should().NotBeNull();
                _browser.Driver.WaitUntilVisible(SummaryPage.ConferenceId(i)).Text.Trim().Should().NotBeNull();
                _browser.Driver.WaitUntilVisible(SummaryPage.CopyButton(i)).Displayed.Should().BeTrue();

                const int expectedNumberOfParticipants = JudgesCount + DefaultData.Individuals + DefaultData.Representatives + DefaultData.Observers + DefaultData.PanelMembers;

                for (var j = 0; j < expectedNumberOfParticipants; j++)
                {
                    _browser.Driver.WaitUntilVisible(SummaryPage.ParticipantUsername(i, j)).Text.Trim().Should().NotBeNull();
                    _browser.Driver.WaitUntilVisible(SummaryPage.ParticipantPassword(i, j)).Text.Trim().Should().NotBeNull();
                }

                for (var j = 0; j < _c.Test.Endpoints; j++)
                {
                    _browser.Driver.WaitUntilVisible(SummaryPage.EndpointDisplayName(i, j)).Text.Trim().Should().StartWith(DefaultData.EndpointsPrefix);
                    _browser.Driver.WaitUntilVisible(SummaryPage.EndpointSipAddress(i, j)).Text.Trim().Should().NotBeNull();
                    _browser.Driver.WaitUntilVisible(SummaryPage.EndpointPin(i, j)).Text.Trim().Should().NotBeNull();
                }
            }
        }

        [Then(@"the user can return to the Create Hearings page")]
        public void ProgressToNextPage()
        {
            _browser.ClickLink(SummaryPage.BackLink);
            _browserSteps.ThenTheUserIsOnThePage("create hearings");
        }
    }
}