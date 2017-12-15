function V = readUnityVectorFile(filepath, dimension)
    if dimension == 2
        formatSpec = '(%f,%f)';
    elseif dimension == 3
        formatSpec = '(%f,%f,%f)';
    elseif dimension == 4
        formatSpec = '(%f,%f,%f,%f)';
    end
    % read contents of file into a string
    fileID = fopen(filepath, 'r');
    C = textscan(fileID, formatSpec);
    V = cell2mat(C)';
    fclose(fileID);
end

