#include <bits/stdc++.h>

using namespace std;

int main(){
    vector <unsigned long long> F;
    F.push_back(0);
    F.push_back(1);
    int pt = 2;
    while(F[pt-1] <= 1000000){
        F.push_back(F[pt-1]+F[pt-2]);
        pt++;
    }
    int t;
    cin >> t;
    while(t--){
        int a, b;
        cin >> a >> b;
        if(a > b) swap(a, b);

        int cnt = 0;
        bool start = false;
        for(int i=0;i<F.size();i++){
            if(F[i] >= a && F[i] <= b && !start){
                cout << F[i] << endl;
                cnt ++;
                start = true;
                continue;
            }
            if(F[i] > b && start){
                break;
            }
            if(start){
                cout << F[i] << endl;
                cnt ++;
            }
        }
        cout << cnt << endl;
        if(t != 0) cout << "------" << endl;
    }
}