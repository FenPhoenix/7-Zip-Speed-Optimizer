# 7-Zip Speed Optimizer

A tool to package 7z FMs in a way that allows FM loaders to scan them quickly, while retaining the high compression advantage of the 7z format.

Performance increase can be up to 11x for AngelLoader (v1.10.0 and later) and up to 70x for FMSel, depending on how conscientiously the FM was previously packaged.

Versions of AngelLoader prior to v1.10.0 gain a lesser advantage, up to 2.5x or so.

File size increase is generally less than 2%, with a small number up to 5% and one observed outlier at 8%. The majority are fractions of a percent larger at most, and a few are actually smaller.