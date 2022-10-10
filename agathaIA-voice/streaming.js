var fs = require('fs');
var twitter = require('twitter');
var sqlite3 = require('sqlite3').verbose();
var voicetext = require('voicetext');
var settings = require('./settings');

var client = new twitter({
  consumer_key: settings.consumer_key,
  consumer_secret: settings.consumer_secret,
  access_token_key: settings.access_token_key,
  access_token_secret: settings.access_token_secret
});

var db = new sqlite3.Database('agathaIA.db');
var voice = new voicetext(settings.voice_text_key);

client.stream('statuses/filter', {track: '@violet-evergardenIA_voice'},  function(stream) {
  stream.on('data', function(tweet) {
    db.serialize(function() {
      db.run('CREATE TABLE IF NOT EXISTS tweets (id_str TEXT PRIMARY KEY, text TEXT, source TEXT, truncated TEXT, in_reply_to_status_id_str TEXT, in_reply_to_user_id_str TEXT, in_reply_to_screen_name TEXT, created_at TEXT, timestamp_ms TEXT)');
      var stmt = db.prepare('INSERT INTO tweets VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)');
      stmt.run(tweet.id_str, tweet.text, tweet.source, tweet.truncated, tweet.in_reply_to_status_id_str, tweet.in_reply_to_user_id_str, tweet.in_reply_to_screen_name, tweet.created_at, tweet.timestamp_ms);
      stmt.finalize();
    });

    voice.speaker(voice.SPEAKER.HIKARI)
      .emotion(voice.EMOTION.HAPPINESS)
      .emotion_level(voice.EMOTION_LEVEL.HIGH)
      .volume(200)
      .speak(tweet.text.replace('@violet-evergarden_voice ', ''), function(e, buf) {
        if (e) {
          console.error(e);
          return;
        }
        fs.writeFile('./public/voices/' + tweet.id_str + '.wav', buf, 'binary', function(e) {
          if (e) {
            console.error(e);
            return;
          }
        });
      });
  });

  stream.on('error', function(error) {
    console.log(error);
  });
});
