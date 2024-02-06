from discord_webhook import DiscordWebhook

token = 'https://discord.com/api/webhooks/1131897123072385055/lEodnTvxFbR2gu36n7c48n9nfuVsmQt6_cTB3NOhW8cx9Z6oJDYvIm8iBPOjJPfWggRO' #token bot

class Discord:
    def __init__(self):
        self.token = token

    def message(self, message):
        webhook = DiscordWebhook(url=token, content=message)
        response = webhook.execute()
        pass
