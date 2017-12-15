addpath('calib');
addpath('util');

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Generate test cases
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
close all
NUM_CASES = 3;
NOISE = 0.02/2;
NOISE_SCALE = eye(4) * NOISE;
NOISE_SCALE(4,4) = 0;
NUM_POSITIONS = 24;
SEED = 5;
THRESHOLD = 1e-5;
MAX_ITERATIONS = 1000;

rng(SEED);
dD = [0;0;0];
uD = [0;1;0];
cDn = zeros(4, NUM_POSITIONS, NUM_CASES);
XDCn = zeros(4, 4, NUM_POSITIONS, NUM_CASES);
XDTn = zeros(4, 4, NUM_CASES);
dTn = zeros(3, NUM_CASES);
sDTn = zeros(1, NUM_CASES);
oVn = zeros(3, NUM_CASES);
hTn = zeros(4, NUM_POSITIONS, NUM_CASES);
RHTn = zeros(3, 3, NUM_POSITIONS, NUM_CASES);
RHVn = zeros(3, 3, NUM_CASES);

for caseIdx = 1 : NUM_CASES
    % generate test case
    cD = [generateCalibrationPositions(0.75, 2, pi-pi/12, pi/12, NUM_POSITIONS); ones(1, NUM_POSITIONS)];
    [XDT, sDT] = randomRigidTransformation(-1000, 1000, 10);
    [RHV, ~] = qr(randn(3));
    oV = 0.2*rand(3, 1) - 0.1;
    
    XDC = zeros(4, 4, NUM_POSITIONS);
    hT = zeros(4, NUM_POSITIONS);
    RHT = zeros(3, 3, NUM_POSITIONS);
    
    dT = XDT * [0;0;0;1]; dT = dT(1:3);
    RDT = XDT(1:3,1:3) / sDT;
    uT = RDT * uD;
    cT = XDT * cD;
    for positionIdx = 1 : NUM_POSITIONS
        oV(3) =  rand(1) - 0.5; % add large depth errors
        XDC(:, :, positionIdx) = lookAt(cD(:, positionIdx), dD, uD);
        RDC = XDC(1:3,1:3,positionIdx);
        XCD = inverseRigidTransformation(XDC(:, :, positionIdx));
        XTC = lookAt(cT(:, positionIdx), dT, uT);
        RTC = XTC(1:3,1:3);
        RHT(:,:, positionIdx) = RTC'*RHV;
        hT(:, positionIdx) = cT(:, positionIdx) + [RHT(:,:,positionIdx)*RHV'*(oV*sDT); 0] + sDT*NOISE_SCALE * randn(4, 1);
    end
    
    cDn(:, :, caseIdx) = cD;
    XDCn(:, :, :, caseIdx) = XDC;
    XDTn(:, :, caseIdx) = XDT;
    dTn(:, caseIdx) = dT;
    sDTn(caseIdx) =  sDT;
    oVn(:, caseIdx) = oV;
    hTn(:, :, caseIdx) = hT;
    RHTn(:, :, :, caseIdx) = RHT;
    RHVn(:, :, caseIdx) = RHV;
end
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% rotation utility Tests
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
for caseIdx = 1 : NUM_CASES
    [R, ~] = qr(randn(3));
    q = rot2quat(R);
    [axis, angle] = rot2axisangle(R);
    Rh1 = axisangle2rot(axis, angle);
    Rh2 = quat2rot(q);
    Rh3 = axisangle2rot(axis * angle);
    err1 = norm(R - Rh1);
    err2 = norm(R - Rh2);
    err3 = norm(R - Rh3);
    if err1 > 1e-3 || err3 > 1e-3
        disp('Error with axis angle!');
    end
    if err2 > 1e-3
        disp('Error with quat!');
    end
end
    
    
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% applyOffset Tests
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% evaluateRotationAndOffset Tests
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% findCenterGeneralized Tests
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% findRigidTransformation Tests
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% findRotation Tests
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% minimizeRotationAndOffset Tests
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% RTD_ERRORS = zeros(2, NUM_CASES);
% tTD_ERRORS = zeros(2, NUM_CASES);
% oV_ERRORS = zeros(2, NUM_CASES);
% FVALS = zeros(2, NUM_CASES);
% RHV_ERRORS = zeros(1, NUM_CASES);
% 
% for caseIdx = 1 : NUM_CASES
%     
% [XTD, oV, fval] =  minimizeRotationAndOffset(hTn(:, :, caseIdx), ...
%                                              cDn(:, :, caseIdx), ...
%                                              XDCn(:, :, :, caseIdx), ...
%                                              1 / sDTn(:, caseIdx));
% [r_err, t_err] = compareTransformationMatrices(XTD, ...
%     inverseRigidTransformation(XDTn(:, :, caseIdx)));
% o_err = norm(oV(1:2) - oVn(1:2, caseIdx));
% 
% RTD_ERRORS(1, caseIdx) = r_err;
% tTD_ERRORS(1, caseIdx) = t_err;
% oV_ERRORS(1, caseIdx) = o_err;
% FVALS(1, caseIdx) = mean(fval);
% 
% [XTD, oV, fval, RHV] =  minimizeRotationAndOffset(hTn(:, :, caseIdx), ...
%                                              cDn(:, :, caseIdx), ...
%                                              XDCn(:, :, :, caseIdx), ...
%                                              1 / sDTn(:, caseIdx), ...
%                                              RHTn(:,: , :, caseIdx));
% 
% [r_err, t_err] = compareTransformationMatrices(XTD, ...
%     inverseRigidTransformation(XDTn(:, :, caseIdx)));
% o_err = norm(oV(1:2) - oVn(1:2, caseIdx));
% 
% RTD_ERRORS(2, caseIdx) = r_err;
% tTD_ERRORS(2, caseIdx) = t_err;
% oV_ERRORS(2, caseIdx) = o_err;
% FVALS(2, caseIdx) = mean(fval);
% RHV_ERRORS(caseIdx) = rotationalSimilarity(RHV, RHVn(:,:,caseIdx)); 
% end
% 
% figure, boxplot(RTD_ERRORS'), title('Rotation errors'), xlabel('DoF'), ylabel('Rotational (RTD) error in radians')
% figure, boxplot(tTD_ERRORS'), title('Translation errors'), xlabel('DoF'), ylabel('Translation error in meters')
% figure, boxplot(oV_ERRORS'), title('Offset errors'), xlabel('DoF'), ylabel('Offset error in meters')
% figure, boxplot(FVALS'), title('FVAL'), xlabel('DoF'), ylabel('mean error in meters')
% figure, boxplot(RHV_ERRORS'), title('RHV errors'), xlabel('DoF'), ylabel('Rotational (RHV) error in meters')

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% unitySolver Tests
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
RTD_ERRORS = zeros(2, NUM_CASES);
tTD_ERRORS = zeros(2, NUM_CASES);
oV_ERRORS = zeros(2, NUM_CASES);
FVALS = zeros(2, NUM_CASES);
RHV_ERRORS = zeros(1, NUM_CASES);

for caseIdx = 1 : NUM_CASES
    calibrationPositionsFilename = strcat('case', num2str(caseIdx), 'CalibrationPositions.txt');
    trackerPositionsFilename = strcat('case', num2str(caseIdx), 'TrackerPositions.txt');
    trackerRotationsFilename = strcat('case', num2str(caseIdx), 'TrackerRotations.txt');
    
    writeUnityVectorFile(calibrationPositionsFilename, reshape(cDn(1:3, :, caseIdx), [1 3*NUM_POSITIONS]), 3);
    writeUnityVectorFile(trackerPositionsFilename, reshape(hTn(1:3, :, caseIdx), [1 3*NUM_POSITIONS]), 3);
    qHTn = zeros(NUM_POSITIONS, 4);
    for i = 1:NUM_POSITIONS
        qHTn(i, :) = rot2quat(RHTn(:,:,i,caseIdx));
    end
    qHTn = qHTn(:, [2 3 4 1]);
    writeUnityVectorFile(trackerRotationsFilename, reshape(qHTn', [1 4*NUM_POSITIONS]), 4);
    
    results3Filename = strcat('3DoFcase', num2str(caseIdx), 'Results.txt');
    results3Filepath = [pwd, filesep, results3Filename];
    calibrationPositionsFilepath = [pwd, filesep, calibrationPositionsFilename];
    trackerPositionsFilepath = [pwd, filesep, trackerPositionsFilename];
    trackerRotationsFilepath = [pwd, filesep, trackerRotationsFilename];
    unitySolver('False', num2str(1/sDTn(caseIdx)), results3Filepath, calibrationPositionsFilepath, trackerPositionsFilepath, trackerRotationsFilepath);
    [XTD, oV, ~, fval] = readResultsFile(results3Filepath);

    [r_err, t_err] = compareTransformationMatrices(XTD, inverseRigidTransformation(XDTn(:, :, caseIdx)));
    o_err = norm(oV(1:2) - oVn(1:2, caseIdx));

    RTD_ERRORS(1, caseIdx) = r_err;
    tTD_ERRORS(1, caseIdx) = t_err;
    oV_ERRORS(1, caseIdx) = o_err;
    FVALS(1, caseIdx) = fval;
    
    results6Filename = strcat('6DoFcase', num2str(caseIdx), 'Results.txt');
    results6Filepath = [pwd, filesep, results6Filename];
    calibrationPositionsFilepath = [pwd, filesep, calibrationPositionsFilename];
    trackerPositionsFilepath = [pwd, filesep, trackerPositionsFilename];
    trackerRotationsFilepath = [pwd, filesep, trackerRotationsFilename];
    unitySolver('True', num2str(1/sDTn(caseIdx)), results6Filepath, calibrationPositionsFilepath, trackerPositionsFilepath, trackerRotationsFilepath);
    [XTD, oV, RHV, fval] = readResultsFile(results6Filepath);

    [r_err, t_err] = compareTransformationMatrices(XTD, inverseRigidTransformation(XDTn(:, :, caseIdx)));
    o_err = norm(oV(1:2) - oVn(1:2, caseIdx));

    RTD_ERRORS(2, caseIdx) = r_err;
    tTD_ERRORS(2, caseIdx) = t_err;
    oV_ERRORS(2, caseIdx) = o_err;
    FVALS(2, caseIdx) = fval;
    RHV_ERRORS(caseIdx) = rotationalSimilarity(RHV, RHVn(:,:,caseIdx)); 
end

figure, boxplot(RTD_ERRORS'), title('Rotation errors'), xlabel('DoF'), ylabel('Rotational (RTD) error in radians')
figure, boxplot(tTD_ERRORS'), title('Translation errors'), xlabel('DoF'), ylabel('Translation error in meters')
figure, boxplot(oV_ERRORS'), title('Offset errors'), xlabel('DoF'), ylabel('Offset error in meters')
figure, boxplot(FVALS'), title('FVAL'), xlabel('DoF'), ylabel('mean error in meters')
figure, boxplot(RHV_ERRORS'), title('RHV errors'), xlabel('DoF'), ylabel('Rotational (RHV) error in meters')
