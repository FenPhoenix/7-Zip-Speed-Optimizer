# 7-Zip Speed Optimizer

A tool to package 7z FMs in a way that allows FM loaders to scan them quickly, while retaining the high compression advantage of the 7z format.

Still in development. Current release versions of AngelLoader cannot yet properly take advantage of this, although an advantage may sometimes be gained depending on how unfavorable the structure of a 7z FM was prior to being repackaged with this tool. Development versions of AngelLoader are achieving an 860% performance increase on average when scanning FMs packed with this tool.
