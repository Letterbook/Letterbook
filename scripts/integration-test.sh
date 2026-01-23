#!/bin/sh
set -e

#
# Run with filter
#
#	./scripts/integration-test.sh --filter CreatePost_Mastodon
#
# Test results go to Tests/Letterbook.IntegrationTests/TestResults/TestResults.trx
dotnet test Tests/Letterbook.IntegrationTests/   \
	--logger "trx;LogFileName=TestResults.trx"   \
	--logger "html;LogFileName=TestResults.html" \
	--logger "GitHubActions" $@
