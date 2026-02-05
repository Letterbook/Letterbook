#!/bin/sh
set -e

dotnet user-secrets set "HostSecret" "$(openssl rand -base64 32)" --project Source/Letterbook

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
