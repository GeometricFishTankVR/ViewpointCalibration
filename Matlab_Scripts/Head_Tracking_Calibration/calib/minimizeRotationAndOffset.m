function [XTD, oV, oVe, RHV] = minimizeRotationAndOffset(hT, cD, XDCn, sTD, RHTn)
n = size(hT, 2);
is6DoF = nargin == 5;
I3 = eye(3);

%% Precompute step
cD = cD(1:3, :); % only need xyz
hT = hT(1:3, :); % only need xyz
% remove the scale from the tracked points
hTs = hT * sTD; % remove the scale

vD = zeros(size(cD)); % Normalized calibration points. (Direction!)
for i = 1:n
    vD(:, i) = cD(:, i) / norm(cD(:, i));
end
Mi = pinv(vD * vD' - n * I3); % This is used in findGeneralizedCenter

%% Compute an initial guess
RTD = findRotation(hTs, cD, mean(hTs, 2), mean(cD, 2));
if is6DoF % Estimate RHV and use as parameter
    qHVn = zeros(n, 4);
    for i = 1:n
        RDC = XDCn(1:3,1:3,i);
        RHT = RHTn(:, :, i);
        qHVn(i, :) = rot2quat(RDC * RTD * RHT);
    end
    q = avg_quaternion_markley(qHVn);
else % Use RTD as parameter
    q = rot2quat(RTD);
end

axis = q(2:4) / norm(q(2:4));
angle = acos(q(1)) * 2;
if angle > pi
    angle = 2 * pi - angle;
end
r = axis * angle;
x0 = [r; 0; 0]; % initial guess

%% Setup and run the optimization
% Set the options
options = optimoptions('fmincon', 'Display', 'none', 'ObjectiveLimit', 1e-3);
A=[]; b=[];Aeq=[];beq=[];
nonlcon = [];

% Set lower and upper bounds of each parameter
ub = [pi; pi; pi; 0.15; 0.15]; lb = -ub;

% Set the function
if is6DoF
    fun = @(x) evaluateRotationAndOffset6DoF(axisangle2rot(x(1:3)), [x(4); x(5); 0], hTs, vD, XDCn, Mi, RHTn);
else
    fun = @(x) evaluateRotationAndOffset3DoF(axisangle2rot(x(1:3)), [x(4); x(5); 0], hTs, vD, XDCn, Mi);
end

% Run the optimization
[xn, ~] = fmincon(fun, x0, A, b, Aeq, beq, lb, ub, nonlcon, options);

%% Extract RTD and oV from the optimizer (xn)
% get the rotation from xn(1:4) [quaternion]
if is6DoF % use RHT to find RHV
    RHV = axisangle2rot(xn(1:3));
    % estimate RTD
    qTDn = zeros(n, 4);
    for i = 1:n
        RCD = XDCn(1:3, 1:3, i)';
        RTH = RHTn(:, :, i)';
        qTDn(i, :) = rot2quat(XDCn(1:3,1:3,i)'*RHV'*RHTn(:,:,i)');
        qTDn(i, :) = rot2quat(RCD * RHV * RTH);
    end
    qTD = avg_quaternion_markley(qTDn);
    RTD = quat2rot(qTD);

    qHVn = zeros(n, 4);
    for i = 1:n
        qHVn(i, :) = rot2quat((XDCn(1:3,1:3,i) * RTD * RHTn(:,:,i)));
    end
    RHV = quat2rot(avg_quaternion_markley(qHVn));
else
    RTD = axisangle2rot(xn(1:3));
    RHV = eye(3);
end
% get the xy componenets of the offset in view from xn(5:6)
oV = [xn(4); xn(5); 0];

%% Compute and evaluate the final transformation
hTD = RTD * hTs;
% find the display in tracker-rotation aligned space
dTD = findCenterGeneralized(hTD, vD, oV, XDCn, Mi);
% Create the full transformation
XTD = [(sTD*I3)*RTD -dTD; 0 0 0 1];
% Transform the tracked points
hD = XTD*[hT; ones(1, size(hT, 2))];
% Evaluate the transformation and offset
oVe = evaluateRotationAndOffset(oV, hD, XDCn);
end

