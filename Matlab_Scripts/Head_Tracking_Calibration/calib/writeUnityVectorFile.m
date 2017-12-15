function writeUnityVectorFile(filename, V, dimension)
    if dimension == 2
        formatSpec = '(%f, %f)\n';
    elseif dimension == 3
        formatSpec = '(%f, %f, %f)\n';
    elseif dimension == 4
        formatSpec = '(%f, %f, %f, %f)\n';
    end
    % read contents of file into a string
    fileID = fopen([pwd, filesep, filename], 'w');
    fprintf(fileID, formatSpec, V);
    fclose(fileID);
end

