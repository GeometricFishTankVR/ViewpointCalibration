function unitySolver(isSixDoF, ...
    trackerToDisplayScaleFactor, ...
    resultsFilePath, ...
    calibrationPositionsFilePath, ...
    trackerPositionsFilePath, ...
    trackerRotationsFilePath)
% UNITYSOLVER Solves for head tracker calibration rigid transformation
% and a viewpoint to head offset
%
%   UNITYSOLVER(isSixDoF, trackerToDisplayScaleFactor, resultsFileName,
%               calibrationPositionsFileName, trackerPositionsFileName,
%               trackerRotationsFileName) 
%   reads in two point sets from text files and calculates the 4x4
%   transformation (XTD) and 3 element vector offset (oV) that best 
%   transforms the tracker points to the calibration points. 
%   The rotations are only read in if isSixDoF is true. A rotation RHV is
%   solved in the 6DoF case to accurately rotate the tracked head
%   orientation to align with the viewers viewing ray. 
%
%   Results are saved in a text file named resultsFileName with the
%   following structure
%
%   XTD11 XTD12 XTD13 XTD14
%   XTD21 XTD22 XTD23 XTD24
%   XTD31 XTD32 XTD33 XTD34
%   XTD41 XTD42 XTD43 XTD44
%   oV.x oV.y oV.z
%   qHV.x qHV.y qHV.z qHV.w
%   err
%
%   where XTD is the transformation matrix, oV is the viewpoint to head
%   displacement vector, and err is the mean displacement error between the
%   positions hC and oV. Since we assume the viewer will be good at
%   aligning the patterns, we assume Calibration space (C) and 
%   View space (V) are the same.
%
%   This file is intended to be compiled using the deploy tool, it
%   can then be run from whichever application is using it as long as the
%   executable is in the directory containing the input files

% Load in the calibration positions in display space (D)
cD = readUnityVectorFile(calibrationPositionsFilePath, 3);
% Load in the head positions in tracker space (T)
hT = readUnityVectorFile(trackerPositionsFilePath, 3);
numPoints = size(cD, 2);

% Generate a series of lookAt matrices from each calibration point,
% looking at the origin, with up being in the positive z direction (to 
% match the display calibration as closely as possible)
% This creates the Transformation from Display space (D) to calibration
% space (C). This transformation will be used interchangebly with XDV
% (Display to View). 
XDC = zeros(4,4,numPoints);
for i = 1:numPoints
    XDC(:,:,i) = lookAt(cD(1:3,i), [0,0,0], [0,1,0]);
end

% Get the scale factor from tracker space to display space. This will be
% used to scale the units into the same unit space.
sTD = str2double(trackerToDisplayScaleFactor);

if strcmp(isSixDoF, 'True')
    % Read in the Head to Tracker orientations
    qHT = readUnityVectorFile(trackerRotationsFilePath, 4);
    qHT = qHT([4 1 2 3], :); %% switch from x y z w convention to w x y z
    % Convert the quaternions to 3x3 rotation matrices
    RHT = zeros(3,3, numPoints);
    for i = 1:numPoints
        RHT(1:3,1:3,i) = quat2rot(qHT(:,i));
    end
    % This is commented out because in the data collected so far there
    % hasn't been any outliers.
    % outlier rejection code:
    %     outlier = 1;
    %     outlierIndices = [];
    %     while size(outlier) > 0
    %         cleanIndices = setdiff(1:numPoints, outlierIndices);
    %         htClean = hT(:, cleanIndices);
    %         cDClean = cD(:, cleanIndices);
    %         XDCClean = XDC(:, :, cleanIndices);
    %         RHTClean = RHT(:,:, cleanIndices);
    %         [XTD, oV, oVe, RHV] = minimizeRotationAndOffset(hT, cD, XDC, sTD, RHT);
    %         [clean,outlier,outlierIndices] = chauvenet(oVe);
    %     end
    
    % Run the 6DoF version by passing in RHT
    [XTD, oV, oVe, RHV] = minimizeRotationAndOffset(hT, cD, XDC, sTD, RHT);
else
    % Leave out RHT and run the 3DoF version
    [XTD, oV, oVe, RHV] = minimizeRotationAndOffset(hT, cD, XDC, sTD);        
end
err = mean(oVe); % Calculate the mean displacement error

% Write out the results to a file
qHV = rot2quat(RHV); qHV = qHV([2 3 4 1]); %% switch back to x y z w convention
% Write the results to text file
outputFile = fopen(resultsFilePath, 'w');
fprintf(outputFile, '%4.8f %4.8f %4.8f %4.8f\n', XTD');
fprintf(outputFile, '%4.8f %4.8f %4.8f\n', oV);
fprintf(outputFile, '%4.8f %4.8f %4.8f %4.8f\n', qHV);
fprintf(outputFile, '%4.8f\n', err);
fclose(outputFile);

%If being run from MATLAB, plot the results
if ~isdeployed
    fprintf('XTD:\n')
    disp(XTD)
    fprintf('\noV:\n')
    disp(oV)
    fprintf('\nRHV:\n')
    disp(RHV)
    disp(rot2quat(RHV))
    fprintf('\nError:\n')
    disp(err)
    
    if size(hT, 1) == 3
        hT = [hT; ones(1, size(hT, 2))];
    end
    % cD, hT, XTD, oV, XDC
    plotCalibrationResults(cD, hT, XTD, oV, XDC)
    figure, title('tracker path'), plot3(hT(1,:), hT(2,:), hT(3,:), 'b-');
 
    oV = [-0.1; -0.05; 0];
    if strcmp(isSixDoF, 'True')
        XDT = inverseRigidTransformation(XTD);
        figure, title('look direction'), hold on
        dT = XDT*[0;0;0;1];
        plot3(dT(1), dT(2),dT(3), 'rx');
        n = size(hT, 2);
        origins = zeros(3, n);
        directions = zeros(3, n);
        for i = 1:size(hT, 2)
            %hT(:, positionIdx) = cT(:, positionIdx) + [RHT(:,:, positionIdx)*RHV'*oV; 0]
            oT = RHT(:,:,i)*RHV'*oV;
            vT = hT(1:3,i) - oT;
            forward = RHT(:,:,i)*RHV'*[0;0;1]/sTD;
            plot3([vT(1) vT(1)+forward(1)], [vT(2) vT(2)+forward(2)], [vT(3) vT(3)+forward(3)], 'g-');
            origins(:, i) = vT(1:3);
            directions(:, i) = forward / norm(forward);
        end
        dT = nLineIntersection(origins, directions);
        plot3(dT(1), dT(2), dT(3), 'bx');
        axis equal;
    end
    
    
% Look at the initial guess
    if size(cD, 1) == 3
        cD = [cD; ones(1, size(cD, 2))];
    end

    if size(hT, 1) == 3
        hT = [hT; ones(1, size(hT, 2))];
    end
    hTscaled = [hT(1:3,:) * sTD; ones(1, size(hT, 2))]; % remove the scale
    % Setup the initial guess
    centroidT = mean(hTscaled(1:3,:), 2);
    centroidD =  mean(cD(1:3,:), 2);
    RTD = findRotation(hTscaled(1:3,:), cD(1:3,:), centroidT, centroidD);
    figure, hold on
    for i = 1:size(hT, 2)
        hD = RTD*hTscaled(1:3, i);
        
        plot3(cD(1, i) - centroidD(1), cD(2, i) - centroidD(2), cD(3, i) - centroidD(3), 'ro')
        plot3(hD(1) - centroidT(1), hD(2) - centroidT(2), hD(3) - centroidT(3), 'bo')
        plot3([cD(1, i) - centroidD(1), hD(1) - centroidT(1)], ...
               [cD(2, i) - centroidD(2), hD(2) - centroidT(2)], ...
               [cD(3, i) - centroidD(3), hD(3) - centroidT(3)], 'g-')
    end
end



