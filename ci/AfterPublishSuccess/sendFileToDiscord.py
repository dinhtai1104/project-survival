from DiscordMessage import Discord
import os
import sys
from urllib.parse import quote
import pathlib

sys.path.insert(1, os.path.join(sys.path[0], '..'))

import Config
import SlackCommand
import subprocess


def run_command(command):
    return subprocess.run(command, stdout=subprocess.PIPE, shell=True).stdout.decode('utf-8')

# git info
commit_time = run_command("git log -1 --pretty=format:%ci")
email = run_command("git log -1 --pretty=format:%ae")
committer = run_command("git log -1 --pretty=format:%cn")
git_full_message = run_command("git log -1 --pretty=format:%B")
git_hash = run_command("git log -1 --pretty=format:%h")

# config
project = Config.read(Config.KEY.PROJECT_NAME)
company = Config.read(Config.KEY.COMPANY_NAME)
slack_channel = SlackCommand.get_channel(Config.read(Config.KEY.SLACK_DEFAULT_CHANNEL))
unity_project = Config.read(Config.KEY.UNITY_PROJECT)
pipeline = Config.read(Config.KEY.PIPELINE)
version_name = Config.read(Config.KEY.BUNDLE_VERSION)
version_code = Config.read(Config.KEY.BUNDLE_VERSION_CODE)

# jenkins
branch = os.environ["BRANCH_NAME"]
build_id = os.environ["BUILD_NUMBER"]
pipeline_url = os.environ["RUN_DISPLAY_URL"]
build_folder = os.path.join(unity_project, "build")
build_url = os.environ["BUILD_URL"]
workspace = os.environ["WORKSPACE"]
artifact_url = build_url + "artifact"
android_identifier = Config.read(Config.KEY.ANDROID_IDENTIFIER)
google_url = f"https://play.google.com/store/apps/details?id={android_identifier}&hl=en_US&gl=US"
# Not use function def due to function not get local variable run in jenkins
for (dirpath, dirnames, filenames) in os.walk(build_folder):
    for file in filenames:
        if file.endswith(".apk"):
            apk_file = os.sep.join([dirpath, file])
            ws = pathlib.Path(workspace)
            p = pathlib.Path(apk_file)            
            full_url =  f"{artifact_url}/{quote('/'.join( p.parts[len(ws.parts):]))}"
            print(full_url)
            msg = f'''\
*{company} | {project}*
{build_id} - {committer} | {branch} | {version_name} | {version_code}
```{git_full_message}```
Unity Build *SUCCESS*
[Console Log]({pipeline_url})
[Download APK]({full_url})
'''             
            Discord().message(message=msg)
            # SlackCommand.send_message(slack_channel, msg)
            #SlackCommand.send_file(slack_channel, apk_file, f"{file}", msg)

        if file.endswith(".aab"):
            aab_file = os.sep.join([dirpath, file])
            ws = pathlib.Path(workspace)
            p = pathlib.Path(aab_file)            
            full_url =  f"{artifact_url}/{quote('/'.join( p.parts[len(ws.parts):]))}"
            print(full_url)
            msg = f'''\
*{company} | {project}*
{git_hash} | {commit_time}
{build_id} - {committer} | {branch} | {version_name} | {version_code}
```{git_full_message}```
Unity PRODUCTION Build *SUCCESS*
[Console Log]({pipeline_url})
[Download ABB]({full_url})
'''
            # SlackCommand.send_message(slack_channel, msg)
            #SlackCommand.send_file(slack_channel, aab_file, f"{file}", msg)
            Discord().message(message=msg)

