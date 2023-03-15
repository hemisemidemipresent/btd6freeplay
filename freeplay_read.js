const fs = require('fs');
const files = fs.readdirSync('./raw/').filter((file) => file.endsWith('.json'));

main();
async function main() {
    read().then((res) => {
        console.log(res);
        fs.writeFile('freeplay.json', JSON.stringify(res), function () {});
    });
}
function read() {
    return new Promise((resolve, vrej) => {
        res = [];
        for (const filename of files) {
            fs.readFile(`./raw/${filename}`, 'utf8', function (err, data) {
                let FBGM = JSON.parse(data); // freeplay bloon group model

                let bounds = FBGM.bounds;

                let obj = { bounds: [] };

                if (bounds[0].upperBounds > 100) {
                    obj.bloon = parseBloons(filename);
                    obj.number = FBGM.bloonEmissions.length;
                    obj.score = FBGM.score;
                    for (i = 0; i < bounds.length; i++) {
                        let bound = bounds[i];
                        let boundset = [bound.lowerBounds, bound.upperBounds];
                        obj.bounds.push(boundset);
                    }
                    res.push(obj);
                }
            });
        }
        setTimeout(() => {
            resolve(res);
        }, 3000);
    });
}
function parseBloons(filename) {
    str = filename.substring(23);
    str = str.replace('.json', '');
    str = str.replace(/\(R\d+\+*\)/, ''); // remove (R120) and related
    str = str.replace(/\[\d+-\d+\],*/g, ''); // remove [201-262], and related
    str = str.replace('(Test)', ''); // remove Test
    str = str.replace('_', ''); // remove underscores
    str = str.replace(/x\d+/g, ''); // remove x1, x2, x3, x5, x120, etc...
    str = str.replace(/\s+/g, '');
    return str;
}
