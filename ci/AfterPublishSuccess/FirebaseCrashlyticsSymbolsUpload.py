import sys
import os
import pathlib

sys.path.insert(1, os.path.join(sys.path[0], '..'))

import subprocess
import Config

def run_command(command):
  return subprocess.run(command, stdout=subprocess.PIPE, shell=True).stdout.decode('utf-8')

def execute_command(cmd, callback):
  for line in iter(os.popen(cmd).readline, ''):
            callback(line[:-1])
def uploadFile():
  unity_project = Config.read(Config.KEY.UNITY_PROJECT)
  firebase_app_id = Config.read(Config.KEY.FIREBASE_ANDROID_APP_ID)
  symbols_file = Config.read(Config.KEY.BUILD_FILE_SYMBOLS)
  symbols_path = os.path.join(unity_project, "build/Android", f"{symbols_file}.zip")
  if os.path.isfile(symbols_path):
      firebase_command = f"firebase crashlytics:symbols:upload --app={firebase_app_id} {symbols_path}"
      print(firebase_command)
      execute_command(firebase_command, print)

branch_name = os.environ["BRANCH_NAME"]
if "production" in branch_name.lower():
  uploadFile()



# for (dirpath, dirnames, filenames) in os.walk(build_folder):
#   for file in filenames:
#     if file.endswith(".zip"):
#       symbols_file = os.sep.join([dirpath, file])
#       symbols_path = pathlib.Path(symbols_file)
#       print(''.join([firebase_command, str(symbols_path)]))
#       execute_command(''.join([firebase_command, str(symbols_path)]), print)
