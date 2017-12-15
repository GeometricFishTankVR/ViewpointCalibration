function [calibrationPoints, trackerPoints, trackerRotation, spherePosition, sphereRadius, isSix] = readSphereeInputs(calibrationFileName, trackerFileName, sphereFileName)
%READINPUTS will read the input files and store the results in matrices
%
%   calibrationPoints and trackerPoints are stored in 4xn matrices where n
%   is the number of points.
%
%   The structure of the input files are assumed to be the same as the
%   Vector3.ToString() function in Unity '(v.x, v.y, v.z)'. If a different
%   convention is desired, this function must be modified for proper
%   reading.

    % Save contents of files to strings
    calibFile = fopen([pwd, filesep, calibrationFileName], 'r');
    trackerFile = fopen([pwd, filesep, trackerFileName], 'r');
    sphereFile = fopen([pwd, filesep, sphereFileName], 'r');
    calibrationString = fscanf(calibFile, '%s');
    trackerString = fscanf(trackerFile, '%s');
    sphereString = fscanf(sphereFile, '%s');
    fclose(sphereFile);
    fclose(calibFile);
    fclose(trackerFile);
    
    % Remove unwanted symbols and separate numbers into cell arrays
    sphereString = strrep(sphereString, '(', '');
    sphereString = strsplit(sphereString, ')');
    sphereRadius = str2double(sphereString{2});
    sphereString = strsplit(sphereString{1}, ',');
    calibrationString = strrep(calibrationString, '(', '');
    calibrationString = strsplit(calibrationString, ')');
    trackerString = strrep(trackerString, '(', '');
    trackerString = strsplit(trackerString, ')');
    c = cell(1,length(calibrationString));
    c(:) = {','};
    calibrationString = cellfun(@strsplit, calibrationString, c, 'UniformOutput', false);
    trackerString = cellfun(@strsplit, trackerString, c, 'UniformOutput', false);
    calibrationString = calibrationString(cellfun(@length, calibrationString) > 1);
    trackerString = trackerString(cellfun(@length, trackerString) > 1);

    % Store numbers into matrices
    numPoints = length(calibrationString);
    calibrationPoints = ones(4, numPoints);
    
    isSix = 0;
    trackerRotation = NaN;
    if length(trackerString) > numPoints
        isSix = 1;
        trackerRotation = zeros(3,numPoints);
        for i = 1:numPoints
            for j = 1:3
                trackerRotation(j, i) = str2double(trackerString{i+numPoints}{j});
            end
        end
    end
    
    trackerPoints = ones(4, numPoints);
    spherePosition = [str2double(sphereString{1}), str2double(sphereString{2}), str2double(sphereString{3})];
    for i = 1:numPoints
        for j = 1:3
            calibrationPoints(j, i) = str2double(calibrationString{i}{j});
            trackerPoints(j, i) = str2double(trackerString{i}{j});
        end
    end
        
end
