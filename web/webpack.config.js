// webpack.config.js
var path = require('path')

module.exports = {
    entry: ['./src/index'], // 在 index 檔案後的 .js 副檔名是可選的
    output: {
        path: path.join(__dirname, 'static'),
        filename: 'bundle.js'
    },
}