function [XTD, oV, RHV, err] = readResultsFile( filepath )
fileID = fopen(filepath, 'r');
row1 = str2double(strsplit(fgetl(fileID), ' '));
row2 = str2double(strsplit(fgetl(fileID), ' '));
row3 = str2double(strsplit(fgetl(fileID), ' '));
row4 = str2double(strsplit(fgetl(fileID), ' '));
XTD = [row1;row2;row3;row4];
oV = str2double(strsplit(fgetl(fileID), ' '))';
qHV =  str2double(strsplit(fgetl(fileID), ' '));
RHV = quat2rot(qHV([4 1 2 3]));
err = str2double(fgetl(fileID));
fclose(fileID);
end

