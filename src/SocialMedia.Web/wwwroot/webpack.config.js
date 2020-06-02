const path = require("path");

module.exports = {
	entry: "./scripts/index.js",
	mode: "production",
	output: {
		filename: "script.js",
		path: path.resolve(__dirname, "scripts"),
	},
	module: {
		rules: [
			{
				test: /\.js$/,
				exclude: /node_modules/,
				loader: "babel-loader",
				options: {
					presets: ["@babel/preset-env"],
				},
			},
		],
	},
};
